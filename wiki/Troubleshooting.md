# Troubleshooting

If Bolt seems to be functioning incorrectly the first steps to try would be to:

1. Ensure you've installed all files from the original Bolt package.
2. Perform a Bolt install: *Edit -> Bolt Install -> Yes*
3. Perform a Bolt compile: *Assets -> Bolt Compile (All)*
4. Restart Unity

If you're still having issues then have a look below if any of it describes your problem and offers a solution. 
If not then feel free to open a topic in the **Support** area of the Bolt Forums.
For the best support it's recommended to include your OS, version of Unity, version of Bolt, a brief description of the issue and if possible the steps to reproduce the problem.

---

###Forgot to install

**Error:**

> NullReferenceException: Object reference not set to an instance of object.  
> BoltEditorGUI.Button( System.String icon)

You've probably installed the Bolt package but didn't perform its internal installation yet.  
Try: *Edit -> Bolt Install -> Yes*

---

###Missing icons in Bolt UI

This is likely a case where you're missing some Bolt files related to its installation.  
Try: *Edit -> Bolt Install -> Yes*

---

###Missing delete button in Bolt Editor UI (Bolt 0.4+)

With Bolt 0.4+ to remove any of the elements in the Bolt Editor you need to **hold control** to have the delete button show up.

---

###The transform is not syncing with the State to other Clients (Bolt 0.4+)

With Bolt 0.4+ ensure to link up the transform in Attached():

    public override void Attached() {
        state.myTransform.SetTransforms(transform);
    }
    
Note that this assumes the transform property on your State is called *myTransform*.
