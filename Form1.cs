using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Listmaker
{
    public partial class Form1 : Form
    {
        byte[] fileBytes;
        string Settings;
        string[] Lines;
        int NoOfLines;
        string DateiPfad;
        string SuchPfad;
        string Directorypath;
        string Dateiname;
        string Zieldatei;
        string Quelldatei;
        string Quelldateiname;
        long qsize;
        int geladen;

        public Form1()
        {
            InitializeComponent();
        }

        private void load_Click(object sender, EventArgs e)
        {
            DialogResult result;
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = "C://" ;
            openFile.Filter = "CSV files (*.csv)|*.csv" ;
            openFile.FilterIndex = 2 ;
            openFile.RestoreDirectory = true ;
            result = openFile.ShowDialog();
            if (result == DialogResult.OK)
                Datei.Text = openFile.FileName;
            if(result == DialogResult.Cancel)
            { Datei.Text = "";return; }
            if (Datei.Text != "")
            { 
                if (File.Exists(Datei.Text))
                {
                DateiPfad = System.IO.Path.GetDirectoryName(Datei.Text);
                SuchPfad=System.IO.Path.GetFullPath(DateiPfad);
                }
                else
                {
                 MessageBox.Show("Datei existiert nicht");
                 return;
                }
            fileBytes=File.ReadAllBytes(Datei.Text);
            geladen = 1;
            }
            else
            {
            MessageBox.Show("Keine Datei eingetragen");
            return;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            geladen = 0;
        }

        private void save_Click(object sender, EventArgs e)
        {
            int i,j,len;
            int Anzahl;
            string t,t1,t2,t3;
            string[] tmp;
            string[] dirarray;
            int ycounter;
            int zcounter;
            int res;
            string tempname;
            string Dateitname;
            Dateiname = Resourcename.Text;

            if (Dateiname=="")
            {
                MessageBox.Show("Kein Resourcennamen eingegeben!");
                return;
            }
            if(geladen==0)
            {
            if(FSDirectory.Text!="")
            {
               if(Directory.Exists(FSDirectory.Text))
               {
                   if(radioButton4.Checked==true)
                   {
                       dirarray = Directory.GetFiles(@FSDirectory.Text);
                       DateiPfad = System.IO.Path.GetDirectoryName(FSDirectory.Text);
                       Zieldatei = FSDirectory.Text+ "\\" + Dateiname + ".c";
                       Anzahl = dirarray.Count();
                       using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Zieldatei, false))
                       {
                           file.WriteLine("//Directory");
                           file.WriteLine("//Directorydatatype");
                           file.WriteLine("//is a structure with the following elements");
                           file.WriteLine("");
                           file.WriteLine("//uint32_t FileLength;");
                           file.WriteLine("//char* FileName;");
                           file.WriteLine("//char* Data;");
                           file.WriteLine("#include \"resource.h\"");
                           for(i=0;i<Anzahl;i++)
                           {
                               Quelldatei = dirarray[i];
                               tempname = Quelldatei;
                               Quelldateiname = Path.GetFileName(@tempname);
                               Dateitname = Path.GetFileNameWithoutExtension(@tempname);
                               FileInfo fi = new FileInfo(@tempname);
                               qsize=fi.Length;
                               file.WriteLine("");                               
                               using (FileStream rfile = new FileStream(tempname, FileMode.Open,FileAccess.Read))
                               {

                                   file.WriteLine("");
                                   t = "__ALIGN_BEGIN_RES char " + Dateitname + "_content";
                                   t = t + "[" + qsize.ToString() + "] RES_FLASH_ATTR __ALIGN_END_RES ={";
                                   file.WriteLine(t);
                                   ycounter = 0;
                                   zcounter = 0;
                                   for (j=0;j<qsize;j++)
                                   {
                                       res = rfile.ReadByte();
                                       if (zcounter!=(qsize-1))
                                       {
                                         t=" 0x" + res.ToString("X2") + ",";
                                       }
                                       else
                                       {
                                           t = " 0x" + res.ToString("X2");
                                       }
                                       file.Write(t);
                                       ycounter++;
                                       zcounter++;
                                       if ((ycounter % 8) == 0) { file.WriteLine(""); }
                                   }
                                   file.WriteLine("");
                                     file.WriteLine("};");
                                   file.WriteLine("");
                               }



                           }
                            
                           for (i = 0; i < Anzahl; i++)
                           {
                               Quelldatei = dirarray[i];
                               tempname = Quelldatei;
                               Quelldateiname = Path.GetFileName(@tempname);
                               Dateitname = Path.GetFileNameWithoutExtension(@tempname);
                               FileInfo fi = new FileInfo(@tempname);
                               qsize = fi.Length;
                               file.WriteLine("");
                               len = Quelldateiname.Length+1;
                               t = "__ALIGN_BEGIN_RES char " + Dateitname + "_name[" + len.ToString() + "] RES_FLASH_ATTR __ALIGN_END_RES=\"" + Quelldateiname + "\0\";";
                               file.WriteLine(t);
                           }
                           file.WriteLine("");
                           t = "__ALIGN_BEGIN_RES Dirdatatype  " + Dateiname + "_list";
                           t = t + "[] RES_FLASH_ATTR __ALIGN_END_RES ={";
                           file.WriteLine(t);
                           for(i=0;i<Anzahl;i++)
                           {
                               Quelldatei = dirarray[i];
                               tempname = Quelldatei;
                               Quelldateiname = Path.GetFileName(@tempname);
                               Dateitname = Path.GetFileNameWithoutExtension(@tempname);
                               FileInfo fi = new FileInfo(@tempname);
                               qsize = fi.Length;
                               t = "["+i.ToString()+"].FileLength=" + qsize.ToString()+",";
                               file.WriteLine(t);
                               t = "[" + i.ToString() + "].FileName=(char*)" + Dateitname + "_name,";
                               file.WriteLine(t);
                               if (i<(Anzahl-1))
                               {
                                   t = "[" + i.ToString() + "].Data=(char*)" + Dateitname + "_content,";
                                file.WriteLine(t);
                                file.WriteLine("");
                               }
                               else
                               {
                                   t = "[" + i.ToString() + "].Data=" + Dateitname + "_content";
                                   file.WriteLine(t);
                               }
                           }
                           file.WriteLine("};");
                           file.WriteLine("");
                           t = "ResourceHandle " + Dateiname + "_resource";
                           t = t + "[] RESOURCE_RES_ATTR  = {";
                           file.WriteLine(t);
                           t = "[0].name=\"" + Dateiname + "\",";
                           file.WriteLine(t);
                           t = "[0].Reourcetype=Resource_Type_Dir,";
                           file.WriteLine(t);
                           t = "[0].RecourceFlag=Resource_Flag_uncompressed,";
                           file.WriteLine(t);
                           t = "[0].resource=(void*)" + Dateiname + "_list ,";
                           file.WriteLine(t);
                           t = "[0].uint16[0]= "+Anzahl.ToString()+",";
                           file.WriteLine(t);
                           t = "[0].uint16[1]= 0";
                           file.WriteLine(t);
                           t = "};";
                           file.WriteLine(t);


                       }
                        MessageBox.Show("Datei gespeichert");
                       return;
                   }
                   MessageBox.Show("Directory gesetzt aber anderer Resourcetyp");
               }
            }
            MessageBox.Show("Keine Datei geladen");
            return;
            }
            Settings = System.Text.Encoding.UTF8.GetString(fileBytes);
            Lines = Settings.Split(new string[] { "\r\n", "" }, StringSplitOptions.RemoveEmptyEntries);
            NoOfLines = Lines.Count();
            Zieldatei = DateiPfad + "\\" + Dateiname + ".c";
            if (radioButton1.Checked == true)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Zieldatei,false))
                {
                    file.WriteLine("//Listdata");
                    file.WriteLine("//Listdatatype");
                    file.WriteLine("//Colorlistdatadatatype is a structure with the following elements");
                    file.WriteLine("");
                    file.WriteLine("//uint8_t name[20]");
                    file.WriteLine("//uint_32 value");
                    file.WriteLine("");
                    file.WriteLine("#include \"resource.h\"");
                    file.WriteLine("");
                    t = "Listdatatype " + Dateiname + "_list";//InfoBlock.face + "_" + InfoBlock.fontSize.ToString();
                    t = t + "[" + (NoOfLines).ToString() + "] RES_FLASH_ATTR ={";
                    file.WriteLine(t);
                    for (i = 0; i < NoOfLines; i++)
                    {
                        t1 = Lines[i];
                        tmp = t1.Split(new string[] { ",", "" }, StringSplitOptions.None);
                        t2 = tmp[0];
                        t3 = tmp[1].Replace("\"", "");
                        t = "[" + i.ToString() + "].name=" + t2 + ",";
                        file.WriteLine(t);
                        t = "[" + i.ToString() + "].uint32=" + t3;
                        if (i < (NoOfLines - 1))
                        {
                            t = t + ",";
                        }
                        file.WriteLine(t);
                    }
                    t = "};";
                    file.WriteLine(t);
                    file.WriteLine("");
                    t = "ResourceHandle " + Dateiname + "_resource";
                    t = t + "[] RESOURCE_RES_ATTR  = {";
                    file.WriteLine(t);
                    t = "[0].name=\"" + Dateiname + "\",";
                    file.WriteLine(t);
                    t = "[0].Reourcetype=Resource_Type_List,";
                    file.WriteLine(t);
                    t = "[0].RecourceFlag=Resource_Flag_uncompressed,";
                    file.WriteLine(t);
                    t = "[0].resource=(void*)" + Dateiname + "_list ,";
                    file.WriteLine(t);
                    t = "[0].uint16[0]= Resource_Listtype_Colors,";
                    file.WriteLine(t);
                    t = "[0].uint16[1]=" + (NoOfLines).ToString() ;
                    file.WriteLine(t);
                    t = "};";
                    file.WriteLine(t);
                }
                MessageBox.Show(Zieldatei+" gespeichert!");
            }

            if (radioButton2.Checked == true)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Zieldatei, false))
                {
                    file.WriteLine("//Listdata");
                    file.WriteLine("//Listdatatype");
                    file.WriteLine("//Colorlistdatadatatype is a structure with the following elements");
                    file.WriteLine("");
                    file.WriteLine("//uint8_t name[20]");
                    file.WriteLine("//char* string");
                    file.WriteLine("");
                    file.WriteLine("#include \"resource.h\"");
                    file.WriteLine("");
                    file.WriteLine("");
                    for (i = 0; i < NoOfLines;i++ )
                    {
                        t1 = Lines[i];
                        tmp = t1.Split(new string[] { ",", "" }, StringSplitOptions.None);
                        t2 = tmp[0];
                        t3 = tmp[1];
                        t = "char " + Dateiname + "_item_" + i.ToString() + "[] RES_FLASH_ATTR =" + t3 + ";";
                        file.WriteLine(t);
                    }
                    file.WriteLine("");
                    file.WriteLine("");
                    t = "Listdatatype " + Dateiname + "_list";//InfoBlock.face + "_" + InfoBlock.fontSize.ToString();
                    t = t + "[" + (NoOfLines).ToString() + "] RES_FLASH_ATTR ={";
                    file.WriteLine(t);
                        for (i = 0; i < NoOfLines; i++)
                        {
                            t1 = Lines[i];
                            tmp = t1.Split(new string[] { ",", "" }, StringSplitOptions.None);
                            t2 = tmp[0];
                            t3 = " &"+Dateiname + "_item_" + i.ToString()+"[0]" ;
                            t = "[" + i.ToString() + "].name=" + t2 + ",";
                            file.WriteLine(t);
                            t = "[" + i.ToString() + "].strings=" + t3;
                            if (i < (NoOfLines - 1))
                            {
                                t = t + ",";
                            }
                            file.WriteLine(t);
                        }
                    t = "};";
                    file.WriteLine(t);
                    file.WriteLine("");
                    t = "ResourceHandle " + Dateiname + "_resource";
                    t = t + "[] RESOURCE_RES_ATTR  = {";
                    file.WriteLine(t);
                    t = "[0].name=\"" + Dateiname + "\",";
                    file.WriteLine(t);
                    t = "[0].Reourcetype=Resource_Type_List,";
                    file.WriteLine(t);
                    t = "[0].RecourceFlag=Resource_Flag_uncompressed,";
                    file.WriteLine(t);
                    t = "[0].resource=(void*)" + Dateiname + "_list ,";
                    file.WriteLine(t);
                    t = "[0].uint16[0]= Resource_Listtype_Strings,";
                    file.WriteLine(t);
                    t = "[0].uint16[1]=" + (NoOfLines).ToString();
                    file.WriteLine(t);
                    t = "};";
                    file.WriteLine(t);
                }
                MessageBox.Show(Zieldatei + " gespeichert!");
            }


            if (radioButton3.Checked == true)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Zieldatei, false))
                {
                    file.WriteLine("//Listdata");
                    file.WriteLine("//Listdatatype");
                    file.WriteLine("//Colorlistdatadatatype is a structure with the following elements");
                    file.WriteLine("");
                    file.WriteLine("//uint8_t name[20]");
                    file.WriteLine("//uint_32 value");
                    file.WriteLine("");
                    file.WriteLine("#include \"resource.h\"");
                    file.WriteLine("");
                    t = "Listdatatype " + Dateiname + "_list";//InfoBlock.face + "_" + InfoBlock.fontSize.ToString();
                    t = t + "[" + (NoOfLines).ToString() + "] RES_FLASH_ATTR ={";
                    file.WriteLine(t);
                    for (i = 0; i < NoOfLines; i++)
                    {
                        t1 = Lines[i];
                        tmp = t1.Split(new string[] { ",", "" }, StringSplitOptions.None);
                        t2 = tmp[0];
                        t3 = tmp[1].Replace("\"", "");
                        t = "[" + i.ToString() + "].name=" + t2 + ",";
                        file.WriteLine(t);
                        t = "[" + i.ToString() + "].uint32=" + t3;
                        if (i < (NoOfLines - 1))
                        {
                            t = t + ",";
                        }
                        file.WriteLine(t);
                    }
                    t = "};";
                    file.WriteLine(t);
                    file.WriteLine("");
                    t = "ResourceHandle " + Dateiname + "_resource";
                    t = t + "[] RESOURCE_RES_ATTR  = {";
                    file.WriteLine(t);
                    t = "[0].name=\"" + Dateiname + "\",";
                    file.WriteLine(t);
                    t = "[0].Reourcetype=Resource_Type_List,";
                    file.WriteLine(t);
                    t = "[0].RecourceFlag=Resource_Flag_uncompressed,";
                    file.WriteLine(t);
                    t = "[0].resource=(void*)" + Dateiname + "_list ,";
                    file.WriteLine(t);
                    t = "[0].uint16[0]= Resource_Listtype_Flags,";
                    file.WriteLine(t);
                    t = "[0].uint16[1]=" + (NoOfLines).ToString();
                    file.WriteLine(t);
                    t = "};";
                    file.WriteLine(t);
                }
                MessageBox.Show(Zieldatei + " gespeichert!");
            }
    
        }

        private void Dirbutton_Click(object sender, EventArgs e)
        {
            var dlg1 = new FolderBrowserDialogEx();
            dlg1.Description = "Select a folder:";
            dlg1.ShowNewFolderButton = true;
            dlg1.ShowEditBox = true;
            //dlg1.NewStyle = false;
            dlg1.SelectedPath = FSDirectory.Text;
            dlg1.ShowFullPathInEditBox = true;
            dlg1.RootFolder = System.Environment.SpecialFolder.MyComputer;

            // Show the FolderBrowserDialog.
            DialogResult result = dlg1.ShowDialog();
            if (result == DialogResult.OK)
            {
                FSDirectory.Text = dlg1.SelectedPath;
            }  
        }

        private void eventLog1_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }
    }
}
