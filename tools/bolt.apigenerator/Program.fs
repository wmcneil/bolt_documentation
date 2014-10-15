// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Mono.Cecil
open FSharp.Data

open System
open System.IO
open System.Xml
open System.Text
open System.Reflection
open System.Text.RegularExpressions
open System.Collections.Generic

///<summary>
///lol
///</summary>
module TypeUtils =  

  let rec classStringName (t:Type) =
    let gt = t.GetGenericArguments()

    if gt.Length = 0 then 
      t.Name

    else
      Regex.Replace(t.Name, "`\d+", "") + "<" + String.Join(", ", gt |> Array.map classStringName) + ">"

open TypeUtils

type Doc = XmlProvider<"C:\\Users\\Fredrik\\Documents\\GitHub\\bolt\\build\\bolt.XML">

type CParam = {
  xml : Doc.Param
} with
  member x.Name = x.xml.Name
  member x.Docs = 
    match x.xml.Value with
    | None -> None
    | Some s -> Some(s :> obj)

type CMethod = {
  xml : Doc.Member
  mtd : Mono.Cecil.MethodDefinition
} with

  member x.Parameters =
    seq { for p in x.xml.Params do yield {CParam.xml = p} }

type CProperty = {
  xml : Doc.Member
  prop : Mono.Cecil.PropertyDefinition
}

type CField = {
  xml : Doc.Member
  field : Mono.Cecil.FieldDefinition
}

type CType = {
  xml : Doc.Member
  typ : Mono.Cecil.TypeDefinition
  mtds : CMethod array
  props : CProperty array
  fields : CField array
}

module Utils =
  let assemblies =
    [
      Mono.Cecil.AssemblyDefinition.ReadAssembly(@"C:\Program Files (x86)\Unity\Editor\Data\Mono\lib\mono\2.0\mscorlib.dll")
      Mono.Cecil.AssemblyDefinition.ReadAssembly("C:\\Users\\Fredrik\\Documents\\GitHub\\bolt\\build\\UnityEngine.dll")
      Mono.Cecil.AssemblyDefinition.ReadAssembly("C:\\Users\\Fredrik\\Documents\\GitHub\\bolt\\build\\udpkit.dll")
      Mono.Cecil.AssemblyDefinition.ReadAssembly("C:\\Users\\Fredrik\\Documents\\GitHub\\bolt\\build\\bolt.dll")
    ]

  let remove regex name =
    Regex.Replace(name, regex, "")
    
  let replace (regex:string) (value:string) (text:string) =
    Regex.Replace(text, regex, value)

  let grab regex text =
    Regex.Match(text, regex).Groups.[1].Value

  let memberName (m:Doc.Member) = 
    m.Name |> grab @"\.([a-zA-Z]+|#ctor)(\(|$|``\d)"

  let findType (name:string) =   
    let mtc = Regex.Match(name, @"(.*?)\{``(\d)\}")

    let name =
      if not mtc.Success then 
        name 
      else
        let n = (int mtc.Groups.[2].Value) + 1
        mtc.Groups.[1].Value + "`" + n.ToString()

    assemblies |> Seq.pick (fun asm ->
      asm.MainModule.Types |> Seq.tryFind (fun t -> t.FullName = name)
    )

  let findClass (m:Doc.Member) =
    let n = 
        let index = m.Name.IndexOf('(')
        if index <> -1 
            then m.Name.Substring(0, index) 
            else m.Name

    let _, count = n |> Seq.countBy (fun c -> c) |> Seq.find (fun (k, c) -> k = '.')
    let regex = sprintf "[TPMEF]:(([a-zA-Z]+(`\d)?\.){%i})" count
    (m.Name |> grab regex).Trim('.') |> findType

  let findProperty (m:Doc.Member) =
    let clss = findClass m
    let name = memberName m
    {
      CProperty.xml = m;
      CProperty.prop = clss.Properties |> Seq.find (fun x -> x.Name = name)
    }
    
  let findField (m:Doc.Member) =
    let clss = findClass m
    let name = memberName m
    {
      CField.xml = m;
      CField.field = clss.Fields |> Seq.find (fun x -> x.Name = name)
    }

  let findMethod (m:Doc.Member) =
    let clss = findClass m
    let name = memberName m

    let ptypes =
      Regex.Match(m.Name, @"\((.*?)\)").Groups.[1].Value.Split(',')
      |> Seq.filter (fun s -> s <> "")
      |> Seq.map (remove "@")
      |> Seq.map findType
      |> Seq.toArray

    let tpcount =
      let mtc = Regex.Match(m.Name, "`(\d+)")
      if mtc.Success then int mtc.Groups.[1].Value else 0

    let isctor =  name = "#ctor" 
    let bind = BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance ||| BindingFlags.Static
    let methods = clss.Methods |> Seq.filter (fun m -> m.IsConstructor = isctor) |> Seq.toArray

    methods |> Seq.pick (fun mtd ->

      let gm = isctor || tpcount = mtd.GenericParameters.Count
      let nm = isctor || (name = mtd.Name)
      let pm =
        let pms = 
          mtd.Parameters
          |> Seq.map (fun p ->
            if p.ParameterType .IsByReference
              then p.ParameterType.GetElementType()
              else p.ParameterType)
          |> Seq.toArray

        if pms.Length = ptypes.Length 
          then pms |> Seq.zip ptypes |> Seq.forall (fun (a, b) -> a.FullName = b.FullName)
          else false

      if gm && nm && pm 
        then Some {CMethod.xml = m; mtd=mtd}
        else None
    )

open Utils

[<EntryPoint>]
let main argv = 

  let sb = 
    new System.Text.StringBuilder();

  let doc = 
    Doc.Parse(System.IO.File.ReadAllText("C:\\Users\\Fredrik\\Documents\\GitHub\\bolt\\build\\bolt.XML"))

  let h2 h = sb.AppendLine("## " + h)
  let h3 h = sb.AppendLine("#### " + h)
  let paragraph s = 
    match s with
    | None -> ()
    | Some v -> sb.AppendLine(Regex.Replace(v, "[\n\r\s\t]+", " ")) |> ignore
  
  let methods = 
    doc.Members
    |> Seq.filter (fun x -> x.Name.StartsWith("M:"))
    |> Seq.map Utils.findMethod
    |> Seq.toArray
  
  let properties =
    doc.Members
    |> Seq.filter (fun x -> x.Name.StartsWith("P:"))
    |> Seq.map Utils.findProperty
    |> Seq.toArray

  let fields =
    doc.Members
    |> Seq.filter (fun x -> x.Name.StartsWith("F:"))
    |> Seq.map Utils.findField
    |> Seq.toArray

  let rec typeName (t:Mono.Cecil.TypeReference) =
    match t.FullName with
    | "System.Void" -> "void"
    | "System.Single" -> "float"
    | "System.Int32" -> "int"
    | "System.UInt32" -> "uint"
    | "System.String" -> "string"
    | "System.Boolean" -> "bool"
    | "System.Object" -> "object"
    | _ -> 
      if t.IsGenericInstance then
        t.FullName |> Utils.remove "`\d+" |> Utils.replace "<" "&lt;" |> Utils.replace ">" "&gt;"

      else
        if t.Namespace = "UnityEngine" 
          then t.Name
          else t.FullName

  let types =
    doc.Members
    |> Seq.filter (fun x -> x.Name.StartsWith("T:"))
    |> Seq.map (fun m ->
      let type' = Utils.findType (m.Name |> remove "^T:")

      let mtds =
        methods 
          |> Seq.filter (fun m -> m.mtd.DeclaringType.FullName = type'.FullName) 
          |> Seq.sortBy (fun m -> if m.mtd.IsStatic then 0 else 1)
          |> Seq.toArray

      let props =
        properties 
          |> Seq.filter (fun p -> p.prop.DeclaringType.FullName = type'.FullName) 
          |> Seq.sortBy (fun p -> if p.prop.GetMethod.IsStatic then 0 else 1)
          |> Seq.toArray

      let fields =
        fields 
          |> Seq.filter (fun p -> p.field.DeclaringType.FullName = type'.FullName) 
          |> Seq.sortBy (fun p -> if p.field.IsStatic then 0 else 1)
          |> Seq.toArray

      {
        CType.xml = m
        CType.typ = type'
        CType.mtds = mtds
        CType.props = props
        CType.fields = fields      
      }
    )
    |> Seq.sortBy (fun t -> t.typ.Name)
    |> Seq.toArray

  sb.AppendLine("# Bolt API Documentation")
  
  for t in types do
    sb.AppendLine(sprintf "[%s](#%s)" t.typ.FullName t.typ.FullName)
  
  for t in types do
    h2 t.typ.FullName
    paragraph t.xml.Summary
    
    for f in t.fields do
      h3 (sprintf "*%s* %s" (typeName f.field.FieldType) f.field.Name)
      paragraph f.xml.Summary

    for p in t.props do
      h3 (sprintf "*%s* %s" (typeName p.prop.PropertyType) p.prop.Name)
      paragraph p.xml.Summary

    for m in t.mtds do
      let parms = String.Join(", ", m.mtd.Parameters |> Seq.map (fun p -> (typeName p.ParameterType) + " " + p.Name))
      h3 (sprintf "%s %s(%s)" (typeName m.mtd.ReturnType) m.mtd.Name parms)

      for p in m.xml.Params do
        match p.Value with
        | None -> paragraph (Some(sprintf "* **%s**" p.Name))
        | Some v -> paragraph (Some(sprintf "* **%s** %s" p.Name v))

      paragraph (Some "")
      paragraph m.xml.Summary

  File.WriteAllText("..\\..\\..\\..\\Api.md", sb.ToString());
  0