// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.

open Mono.Cecil

open System
open System.IO
open System.Xml
open System.Text
open System.Reflection
open System.Text.RegularExpressions
open System.Collections.Generic

let remove regex name =
  Regex.Replace(name, regex, "")
    
let replace (regex:string) (value:string) (text:string) =
  Regex.Replace(text, regex, value)

let grab regex text =
  Regex.Match(text, regex).Groups.[1].Value

[<EntryPoint>]
let main argv = 

  let rec typeNamePretty (t:Mono.Cecil.TypeReference) =
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
        t.FullName |> remove "`\d+" |> replace "<" "&lt;" |> replace ">" "&gt;"

      else
        if t.Namespace = "UnityEngine" 
          then t.Name
          else t.FullName

  let sb = ref (new System.Text.StringBuilder())
  let h1 h = (!sb).AppendLine("# " + h) |> ignore
  let h2 h = (!sb).AppendLine("## " + h) |> ignore
  let h3 h = (!sb).AppendLine("### " + h) |> ignore
  let h4 h = (!sb).AppendLine("#### " + h) |> ignore

  let p s = 
    (!sb).AppendLine(Regex.Replace(s, "[\n\r\s\t]+", " ")) |> ignore
    
  let code s = 
    (!sb).AppendLine("`" + Regex.Replace(s, "[\n\r\s\t]+", " ") + "`") |> ignore

  let makePath file =
    let path = System.IO.Path.GetFullPath("..\\..\\..\\..\\Api\\" + file)
    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path)) |> ignore
    path

  let save file =
    File.WriteAllText(makePath file, (!sb).ToString());

  let saveIfNotExists file =
    if not (File.Exists(makePath file)) then
      File.WriteAllText(makePath file, (!sb).ToString());

  let includeFile header file =
    h2 header

    if File.Exists(makePath file) then
      (!sb).Append(File.ReadAllText(makePath file)) |> ignore

    else
      p (sprintf "Missing File '%s'" file)

  let dll = Mono.Cecil.AssemblyDefinition.ReadAssembly("C:\\Users\\Fredrik\\Documents\\GitHub\\bolt\\build\\bolt.dll")

  let types = 
    dll.MainModule.Types 
    |> Seq.filter (fun t -> t.IsPublic)
    |> Seq.filter (fun t -> t.CustomAttributes |> Seq.exists (fun a -> a.AttributeType.FullName = "Bolt.DocumentationAttribute"))

  let typeName (t:Mono.Cecil.TypeDefinition) =
    let attr = t.CustomAttributes |> Seq.find (fun a -> a.AttributeType.FullName = "Bolt.DocumentationAttribute")
    let alias = attr.Properties |> Seq.tryFind (fun p -> p.Name = "Alias")
    
    match alias with
    | None -> t.FullName
    | Some(v) -> v.Argument.Value.ToString()

  let fields (t:Mono.Cecil.TypeDefinition) = t.Fields |> Seq.filter (fun f -> f.IsPublic)
  let properties (t:Mono.Cecil.TypeDefinition) = t.Properties |> Seq.filter (fun p -> p.GetMethod.IsPublic)
  let methods (t:Mono.Cecil.TypeDefinition) = 
    t.Methods 
    |> Seq.filter (fun m -> m.IsPublic && m.Name <> ".ctor")
    |> Seq.filter (fun m -> (not <| m.Name.StartsWith("get_")) && (not <| m.Name.StartsWith("set_")))
  
  let typePath (t:Mono.Cecil.TypeDefinition) = sprintf "Types/%s.md" (typeName t)
  let typeLink (t:Mono.Cecil.TypeDefinition) = sprintf "[%s](%s)" (typeName t) (typePath t)
  let typeInclude header (t:Mono.Cecil.TypeDefinition) = 
    includeFile header (t |> typePath |> replace ".md" ("_" + header + ".md"))

  let memberPath (m:Mono.Cecil.IMemberDefinition) = 
    let memberTypeName = m.GetType().Name.[0].ToString()
    sprintf "Types/%s/%s/%s.md" (typeName m.DeclaringType) memberTypeName m.Name

  let memberLink (m:Mono.Cecil.IMemberDefinition) = 
    sprintf "[%s](%s)" m.Name (memberPath m)
    
  let memberInclude header (m:Mono.Cecil.IMemberDefinition)  = 
    includeFile header (m |> memberPath |> replace ".md" ("_" + header + ".md"))

  let writeReadme () =
    sb := new System.Text.StringBuilder()

    h1 "Bolt API Documentation"

    for t in types do
      h3 (sprintf "[%s](%s.md)" (typeName t) (typeName t))

    save "README.md"

  let writeTypes () =
  
    for t in types do
      sb := new System.Text.StringBuilder()

      h1 (typeName t)

      t |> typeInclude "Summary"
      t |> typeInclude "Example"

      h2 "Fields"
      for f in fields t do h3 (memberLink f)
      
      h2 "Properties"
      for p in properties t do h3 (memberLink p)
      
      h2 "Methods"
      for m in methods t do h3 (memberLink m)

      save (typePath t)

  let writeMembers () =
    let isStatic yes =
      if yes then "static " else ""

    for t in types do

      for m in fields t do
        sb := new System.Text.StringBuilder()

        h1 (sprintf "%s.%s" (typeLink t) (m.Name))
        
        let typ = typeNamePretty m.FieldType
        let statc = isStatic m.IsStatic
        let readonly = if m.IsInitOnly then "readonly " else ""

        code (sprintf "public %s%s%s %s" readonly statc typ m.Name)
        
        m |> memberInclude "Summary"
        m |> memberInclude "Example"

        save (memberPath m)

      for m in properties t do
        sb := new System.Text.StringBuilder()

        h1 (sprintf "%s.%s" (typeLink t) (m.Name))

        let set = if m.SetMethod <> null then "set; " else ""
        let typ = typeNamePretty m.PropertyType
        let statc = isStatic m.GetMethod.IsStatic
        code (sprintf "public %s%s %s { get; %s}" statc typ m.Name set)
        
        m |> memberInclude "Summary"
        m |> memberInclude "Example"

        save (memberPath m)

      for m in methods t do
        sb := new System.Text.StringBuilder()

        h1 (sprintf "%s.%s" (typeLink t) (m.Name))

        let ret = typeNamePretty m.ReturnType

        let modif = 
          if m.IsStatic then "static "
          elif m.IsVirtual then "virtual "
          else ""

        let parameters = 
          String.Join(", ", m.Parameters |> Seq.map (fun p -> (typeNamePretty p.ParameterType) + " " + p.Name))

        code (sprintf "public %s%s %s(%s)" modif ret m.Name parameters)
        
        if m.Parameters.Count > 0 then
          m |> memberInclude "Parameters"

        m |> memberInclude "Summary"
        m |> memberInclude "Example"

        save (memberPath m)


  writeReadme() 
  writeTypes()
  writeMembers()

//  
//  sb.AppendLine("");
//  sb.AppendLine("");
//
//  for t in types do
//    h2 t.typ.FullName
//    paragraph t.xml.Summary
//    
//    for f in t.fields do
//      h3 (sprintf "*%s* %s" (typeName f.field.FieldType) f.field.Name)
//      paragraph f.xml.Summary
//
//    for p in t.props do
//      h3 (sprintf "*%s* %s" (typeName p.prop.PropertyType) p.prop.Name)
//      paragraph p.xml.Summary
//
//    for m in t.mtds do
//      let parms = String.Join(", ", m.mtd.Parameters |> Seq.map (fun p -> (typeName p.ParameterType) + " " + p.Name))
//      h3 (sprintf "%s %s(%s)" (typeName m.mtd.ReturnType) m.mtd.Name parms)
//      paragraph m.xml.Summary

  0