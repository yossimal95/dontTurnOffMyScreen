using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dontTurnOffMyScreen
{
    public partial class Form1 : Form
    {
        //enum
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
            // Legacy flag, should not be used.
            // ES_USER_PRESENT = 0x00000004
        }

        // method
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);


        static void PreventSleep()
        {
            // Prevent Idle-to-Sleep (monitor not affected) (see note above)
            SetThreadExecutionState(
            EXECUTION_STATE.ES_CONTINUOUS |
            EXECUTION_STATE.ES_DISPLAY_REQUIRED |
            EXECUTION_STATE.ES_SYSTEM_REQUIRED
            );
        }

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "שומר אותך ער..";
        }
        static bool toSave;

        static bool MouseMove = false;

        static Thread t = new Thread(() => {

            Random rn = new Random();
            while (toSave)
            {
                if (MouseMove)
                {
                    Cursor.Position = new System.Drawing.Point(rn.Next(0, 1000), rn.Next(0, 1000));
                }                
                PreventSleep();
                Thread.Sleep(10000);
            }
            t.Abort();
        });


        private void button1_Click(object sender, EventArgs e)
        {
            toSave = true;

            label1.ForeColor = Color.Green;
            label1.Text = "מצב: דולק";

            t.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label1.ForeColor = Color.Red;
            label1.Text = "מצב: כבוי";

            toSave = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (toSave)
            {
                MessageBox.Show("נא ללחוץ על כפתור הכיבוי לפני סגירה");
                e.Cancel = true;
            }
            else
            {
                t.Abort();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            MouseMove = MouseMove == true ? false : true;
        }

    }
}
