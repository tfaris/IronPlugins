"""
Creates a simple windows form with a text box that executes any
arbitrary IronPython code typed in by the user. This allows
instant feedback and live application modification by the user
at runtime.
"""

import clr
clr.AddReference("System.Windows.Forms")
clr.AddReference("System.Drawing")
clr.AddReference("IronPlugins")
from System import Exception
from System.Windows.Forms import *
from System.Drawing import *
from IronPlugins.Plugin import PythonPluginException

f = Form()
f.Size = Size(300,300)

t = TextBox()
t.Size = Size(f.Width,f.Height-100)
t.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
t.Multiline = True

def clicked(sender,e):
    try:
        code = t.Text
        # Python only likes the "\n" style newlines
        code = code.replace("\r\n","\n")
        exec(code)        
    except Exception,e:
        MessageBox.Show(PythonPluginException(e).Message)

b = Button()
b.Click += clicked
b.Text = "Run"
b.Anchor = AnchorStyles.Right | AnchorStyles.Bottom

f.Show()
f.Controls.Add(t)
f.Controls.Add(b)
b.Location = Point(f.Width-b.Width - 20,t.Bottom+b.Height)
