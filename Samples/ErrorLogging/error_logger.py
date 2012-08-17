import clr,random
clr.AddReference("System")
clr.AddReference("System.Drawing")
clr.AddReference("System.Windows.Forms")

import System.Windows.Forms as Forms
from System.Diagnostics import Trace,DefaultTraceListener
from System.Drawing import Size,Point,Color,Font

class RichTraceListener(DefaultTraceListener):
    """
    Displays trace information randomly sized and colored in a
    RichTextBox. This is trivial, but real uses could be logging
    messages to a database or to a text file, etc.
    """
    def __init__(self):
        f = Forms.Form()
        f.Size = Size(300,300)
        f.FormClosed += lambda sender,e:Trace.Listeners.Remove(self)

        t = Forms.RichTextBox()
        t.Dock = Forms.DockStyle.Fill
        f.Controls.Add(t)
        t.SelectionBackColor = Color.SkyBlue
        t.AppendText("Now we'll log some errors!\r\n")
        t.ReadOnly = True

        def text_changed(sender,e):
            self.text.SelectionStart = len(self.text.Text)
            self.text.ScrollToCaret()
        t.TextChanged += text_changed

        f.Show()
        self.form = f
        self.text = t

    def WriteLine(self, message):
        self.text.SelectionBackColor = Color.FromArgb(random.randint(0,255),random.randint(0,255),random.randint(0,255))
        self.text.SelectionFont = Font(self.text.Font.FontFamily, random.randint(8,42), self.text.Font.Style)
        self.text.AppendText(message+"\r\n")        

Trace.Listeners.Add(RichTraceListener())