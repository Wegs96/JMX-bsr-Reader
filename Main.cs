using System;
using System.IO;
using System.Windows.Forms;

namespace JMXbsrFile
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            treeView1.Visible = false;

        }

        public void teeview()
        {
            if (treeView1.Nodes.Count == 0)
            {
                treeView1.Visible = false;
                treeView1.Nodes.Add("0", "");
                treeView1.Nodes[0].Nodes.Add("0", "Materials");
                treeView1.Nodes[0].Nodes.Add("1", "Meshes");
                treeView1.Nodes[0].Nodes[1].Nodes.Add("0", "");
                treeView1.Nodes[0].Nodes.Add("2", "Skeleton");
                treeView1.Nodes[0].Nodes.Add("3", "Animations");
            }
            else
            {
                treeView1.Nodes.Clear();
                treeView1.Visible = false;
                treeView1.Nodes.Add("0", "");
                treeView1.Nodes[0].Nodes.Add("0", "Materials");
                treeView1.Nodes[0].Nodes.Add("1", "Meshes");
                treeView1.Nodes[0].Nodes[1].Nodes.Add("0", "");
                treeView1.Nodes[0].Nodes.Add("2", "Skeleton");
                treeView1.Nodes[0].Nodes.Add("3", "Animations");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog bsrfile = new OpenFileDialog();
                bsrfile.Filter = @"Joymax bsr Files (*.bsr) | *.bsr";
                bsrfile.FilterIndex = 1;
                bsrfile.Multiselect = false;
                if (bsrfile.ShowDialog() == DialogResult.OK)
                {
                    teeview();
                    string bsrpath = bsrfile.FileName;

                    FileStream stream = new FileStream(bsrpath,FileMode.Open);
                    using (BinaryReader read = new BinaryReader(stream))
                    {
                        char[] Format = read.ReadChars(12);
                        fFormat.Text = ToString(Format);
                      
                        //pointers

                        #region Pointers

                        uint pMaterial = read.ReadUInt32();
                        uint pMesh = read.ReadUInt32();
                        uint pSkeleton = read.ReadUInt32();
                        uint pAnimation = read.ReadUInt32();
                        uint pMeshGroup = read.ReadUInt32();
                        uint pAnimationGroup = read.ReadUInt32();
                        uint pSoundEffect = read.ReadUInt32();
                        uint pBoundingBox = read.ReadUInt32();

                        #endregion
                        //flags

                        #region Flags

                        uint funkUInt0 = read.ReadUInt32();
                        uint funkUInt1 = read.ReadUInt32();
                        uint funkUInt2 = read.ReadUInt32();
                        uint funkUInt3 = read.ReadUInt32();
                        uint funkUInt4 = read.ReadUInt32();

                        #endregion
                        //......
                        string type = ToHex(read.ReadUInt32());
                        int namelen = read.ReadInt32();
                        char[] name = read.ReadChars(namelen);
                        treeView1.Nodes[0].Text += ToString(name);  // bsr name
                        byte[] unkBuffer = new byte[48];
                        unkBuffer = read.ReadBytes(unkBuffer.Length);

                        //-------Read Material Files--------\\

                        #region Materials

                        read.BaseStream.Position = pMaterial;
                        uint materialcount = read.ReadUInt32();
         
                        for (int material = 0; materialcount > material; material++)
                        {
                            uint MaterialID = read.ReadUInt32();
                            int MaterialPathLen = read.ReadInt32();
                            char[] MaterialPath = read.ReadChars(MaterialPathLen);
                            treeView1.Nodes[0].Nodes[0].Nodes.Add(Convert.ToString(materialcount), new string(MaterialPath));
                        }

                        #endregion
                        //-------Read Mesh Files--------\\

                        #region Meshes

                        read.BaseStream.Position = pMesh;
                        uint MeshCount = read.ReadUInt32();
                        for (int meshindex = 0; MeshCount > meshindex; meshindex++)
                        {
                            int MeshNameLen = read.ReadInt32();
                            char[] MeshName = read.ReadChars(MeshNameLen);
                            if (funkUInt0 == 1)
                            {
                                uint meshUnkUInt0 = read.ReadUInt32();
                            }
                            treeView1.Nodes[0].Nodes[1].Nodes[0].Nodes.Add(Convert.ToString(meshindex), new string(MeshName));
                        }

                        #endregion

                        //-------Read Animation Files--------\\

                        #region Animation

                        read.BaseStream.Position = pAnimation;
                        uint unkUInt5 = read.ReadUInt32();
                        uint unkUInt6 = read.ReadUInt32();
                        uint animationCount = read.ReadUInt32();
                        for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
                        {
                            int animationPathLen = read.ReadInt32();
                            char[] animationPath = read.ReadChars(animationPathLen);
                            treeView1.Nodes[0].Nodes[3].Nodes.Add(Convert.ToString(animationIndex),new string(animationPath));
                        }

                        #endregion

                        //-------Read Skeleton Files--------\\

                        #region Skeleton

                        read.BaseStream.Position = pSkeleton;
                        uint skeletonCount = read.ReadUInt32();
                        for (int skeletonIndex = 0; skeletonIndex < skeletonCount; skeletonIndex++)
                        {
                            int skeletonPathLen = read.ReadInt32();
                            char[] skeletonPath = read.ReadChars(skeletonPathLen);
                            int extraByteCount = read.ReadInt32();
                            byte[] skeletonExtraBytes = read.ReadBytes(extraByteCount);
                            treeView1.Nodes[0].Nodes[2].Nodes.Add(Convert.ToString(skeletonIndex), new string(skeletonPath));
                        }
                        

                        #endregion
                        //-------Read MeshGroups--------\\

                        #region MeshGroups

                        read.BaseStream.Position = pMeshGroup;
                        uint meshGroupCount = read.ReadUInt32();
                        for (int i = 0; i < meshGroupCount; i++)
                        {
                            int meshGroupNameLen = read.ReadInt32();
                            char[] meshGroupName = read.ReadChars(meshGroupNameLen);
                            uint meshFileCount = read.ReadUInt32();
                            
                            treeView1.Nodes[0].Nodes[1].Nodes[0].Text = new string(meshGroupName);
                            for (int ii = 0; ii < meshFileCount; ii++)
                            {
                                uint meshFileIndex = read.ReadUInt32();
                            }
                        }
                        #endregion

                        //-------Read Animation Groups--------\\

                        #region ani groups

                        /*
                      
                         //pointer.AnimationGroup will get you here
4   uint    animationGroupCount
for (int i = 0; i < animationGroupCount; i++)
{       
    4   uint    groupName.Length
    4   uint    groupName

    4   uint    animationCount
    for (int ii = 0; ii < entryCount; ii++)
    {
        4   uint    animation.Type          //see ResourceAnimationType
        4   uint    animation.FileIndex

        4   uint    eventCount
        for (int iii = 0; iii < eventCount; iii++)
        {
            4   uint    event.KeyTime
            4   uint    event.Type
            4   uint    event.unkValue0            
            4   uint    event.unkValue1
        }

        4   uint    walkGraphPointCount
        4   float   animation.WalkLength
        for (int iiii = 0; iiii < eventCount; iiii++)
        {
            8   Vector2 walkGraphPoint
        }
    }
}
                         
                         */
                        #endregion

                        treeView1.Visible = true;
                        treeView1.ExpandAll();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public static string ToString(char[] chars)
        {
            return new string(chars);
        }
        public static string ToHex(uint value)
        {
            return String.Format("0x{0:X}", value);
        }
    }
}
