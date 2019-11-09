using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace project_big
{
    public partial class Form1 : Form
    {
        private Bitmap ImageFile;

        private int CELL_SIZE = 60;
        private int PIECE_COUNT = 4;

        private const int CELL_SIZE_1 = 60;
        private const int PIECE_COUNT_1 = 4;

        private const int CELL_SIZE_2 = 40;
        private const int PIECE_COUNT_2 = 6;

        private const int CELL_SIZE_3 = 30;
        private const int PIECE_COUNT_3 = 8;

        private PictureBox[] picCell = null;
        // tọa độ để xác định vị trí các khung ảnh sẽ tạo ra
        private const int OffSetX = 300;
        private const int OffSetY = 0;
        /// <summary>
        /// Vị trí chuột trên khung ảnh khi bắt đầu khi bắt đầu drag
        /// Dùng để điều chỉnh vị trí chuột luôn tương đối với khung ảnh
        /// </summary>
        private Point startDragPoint;

        /// <summary>
        /// Vị trí khung ảnh gốc khi bắt đầu drag
        /// dùng để hoán đổi vị trí ảnh nếu 2 ảnh mới đè lên ảnh cũ 
        /// </summary>
        private Point picLocation;


        public Form1()
        {
            InitializeComponent();
            panel2.Paint += new PaintEventHandler(DrawGrid);
            //PaintFrames();           
        }

        private void PaintFrames()
        {            
            panel2.Refresh(); 
            panel1.Refresh();    
            picCell = new PictureBox[PIECE_COUNT * PIECE_COUNT];
            for (int i = 0; i < picCell.Length; i++)
            {
                picCell[i] = new PictureBox();
                picCell[i].MouseUp += new MouseEventHandler(picCell_MouseUp);
                picCell[i].MouseMove += new MouseEventHandler(picCell_MouseMove);
                picCell[i].MouseDown += new MouseEventHandler(picCell_MouseDown);
                panel1.Controls.Add(picCell[i]);
            }
            // Thêm sự kiện vẽ lưới vào sự kiện Paint của panel Image
            panel2.Paint += new PaintEventHandler(DrawGrid);
            
        }       
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.RestoreDirectory = true;
            dlgOpen.Filter = "JPEG Files(*.jpg)|*.jpg|Bitmap Files(*.bmp)|*.bmp|GIF Files(*.gif)|*.gif|All Files|*.*";
            dlgOpen.FilterIndex = 1;
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                LoadPicture(dlgOpen.FileName);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadPicture(Application.StartupPath + "\\myImage.jpg");
        }
        
        private void picCell_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            // Di chuyển khung ảnh theo chuột khi kéo
            if (e.Button == MouseButtons.Left)
            {
                pic.Location = new Point(pic.Left + e.X - startDragPoint.X,
                    pic.Top + e.Y - startDragPoint.Y);
            }
            // Tính vị trí dòng và cột với đơn vị là 1 CELL_SIZE
            int col = pic.Location.X / CELL_SIZE;
            int row = pic.Location.Y / CELL_SIZE;
            // Nếu nằm ngoài panel Image thì thoát hàm
            if (col >= PIECE_COUNT || row >= PIECE_COUNT)
                return;
            // Vẽ đường biên màu đỏ xác định vị trí mới của ảnh trên panel
            Graphics g = panel2.CreateGraphics();
            g.DrawRectangle(Pens.Red, new Rectangle(col * CELL_SIZE, row * CELL_SIZE, CELL_SIZE, CELL_SIZE));
        }

        private void picCell_MouseDown(object sender, MouseEventArgs e)
        {
            // Khi nhấn chuột và khung ảnh
            // ta sẽ lưu vị trí nhấn chuột và vị trí khung ảnh lại
            PictureBox pic = (PictureBox)sender;
            startDragPoint = e.Location;

            picLocation = pic.Location;
            // Đưa khung ảnh lên trên cùng để ko bị che mất
            pic.BringToFront();

        }
        // thả chuột 
        private void picCell_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pic = (PictureBox)sender;
            // Tính vị trí mới của ảnh khít với dòng, cột trên panel
            int col = (pic.Location.X / CELL_SIZE);
            int row = (pic.Location.Y / CELL_SIZE);
            if (col >= PIECE_COUNT || row >= PIECE_COUNT)
                return;
            // Lấy control tại ví trí mới
            Control ctl = panel1.GetChildAtPoint(new Point(col * CELL_SIZE, row * CELL_SIZE));
            // Nếu đã có một khung ảnh tại ô này
            // thì chuyển vị trí của khung ảnh này về vị trí của khung ảnh vừa drop
            if (ctl != null && ctl is PictureBox)
            {
                ctl.Location = picLocation;
            }
            // Gán vị trí mới cho khung ảnh
            pic.Location = new Point(col * CELL_SIZE, row * CELL_SIZE);

            if (CheckWin)
            {
                MessageBox.Show("Finish!");
            }

        }
        private bool CheckWin
        {
            // Phương pháp:
            // Lặp qua tất cả các cột và dòng trong panel Image
            // Lấy khung ảnh tại vị trí dòng, cột đó
            // Nếu khung ảnh ko tồn tại trả về false
            // Nếu khung ảnh có vị trí sai thứ tự trả về false
            get
            {
                for (int i = 0; i < PIECE_COUNT; i++)
                {
                    for (int j = 0; j < PIECE_COUNT; j++)
                    {
                        // Tính thứ tự đúng của khung ảnh ở vị trí hiện tại
                        int index = i * PIECE_COUNT + j;
                        // Lấy control tại vị trí hiện tại
                        Control ctl = panel1.GetChildAtPoint(new Point(j * CELL_SIZE, i * CELL_SIZE));
                        // Nếu khung ảnh ko tồn tại trả về false
                        if (ctl == null || !(ctl is PictureBox))
                            return false;
                        // Nếu khung ảnh có vị trí sai trả về false
                        if (ctl.Tag.ToString() != index.ToString())
                            return false;
                    }
                }
                return true;
            }
        }
        void DrawGrid(object obj, PaintEventArgs pe)
        {
            Graphics g = panel2.CreateGraphics();
            Pen p = Pens.White;
            int length = CELL_SIZE * PIECE_COUNT;
            for (int i = 0; i <= PIECE_COUNT; i++)
            {
                int pos = CELL_SIZE * i;
                // Vẽ lưới ngang
                Point p1 = new Point(0, pos);
                Point p2 = new Point(length, pos);
                g.DrawLine(p, p1, p2);
                // Vẽ lưới dọc
                p1 = new Point(pos, 0);
                p2 = new Point(pos, length);
                g.DrawLine(p, p1, p2);
            }

        }

        private void LoadPicture(string fileName)
        {
            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show("Không tìm thấy file \n" + fileName);
                return;
            }
            try
            {
                ImageFile = null;
                ImageFile = new Bitmap(Bitmap.FromFile(fileName), panel2.Size);
                int imageSize = panel2.Width;
                imageSize = imageSize - imageSize % PIECE_COUNT;
                // Tạo và sắp xếp các khung ảnh
                InitPictures();
                // Xáo trộn các khung ảnh
                toolStripMenuItem3_Click(null, null);                              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void InitPictures()
        {           
            if (ImageFile != null)
            {
                int index = 0;                
                for (int j = 0; j < PIECE_COUNT; j++)
                {
                    int posY = j * CELL_SIZE;
                    for (int i = 0; i < PIECE_COUNT; i++)
                    {
                        try
                        {
                            Rectangle imageRect = new Rectangle(i * CELL_SIZE, posY, CELL_SIZE, CELL_SIZE);
                            picCell[index].Image = null;
                            picCell[index].Image = ImageFile.Clone(imageRect, PixelFormat.DontCare);                            
                            picCell[index].Location = new Point(OffSetX + i * CELL_SIZE, OffSetY + posY + 1);
                            picCell[index].Size = new Size(CELL_SIZE, CELL_SIZE);
                            picCell[index].Tag = index.ToString();                            
                            index++;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }            
            // Hiển thị ảnh kết quả vào picture1
            pictureBox1.Image = ImageFile;

        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {            
            Bitmap bmp;
            string temptag;
            Random rnd = new Random();

            int maxValue = PIECE_COUNT * PIECE_COUNT;

            for (int i = 0; i < maxValue; i++)
            {
                int indexSource = rnd.Next(maxValue);
                int indexDest = rnd.Next(maxValue);
                if (indexSource == indexDest)
                    continue;
                try
                {
                    bmp = (Bitmap)picCell[indexSource].Image;
                    temptag = picCell[indexSource].Tag.ToString();
                    picCell[indexSource].Image = picCell[indexDest].Image;
                    picCell[indexSource].Tag = picCell[indexDest].Tag;
                    picCell[indexDest].Image = bmp;
                    picCell[indexDest].Tag = temptag;
                    this.Invalidate();
                }
                catch (Exception)
                {
                }

            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            DeleteImage();
            CELL_SIZE = CELL_SIZE_1;
            PIECE_COUNT = PIECE_COUNT_1;            
            PaintFrames();
            Form1_Load(null, null);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            DeleteImage();
            CELL_SIZE = CELL_SIZE_2;
            PIECE_COUNT = PIECE_COUNT_2;           
            PaintFrames();
            Form1_Load(null, null);                      
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            DeleteImage();
            CELL_SIZE = CELL_SIZE_3;
            PIECE_COUNT = PIECE_COUNT_3;
            PaintFrames();
            Form1_Load(null, null);
        }

        //--------------------------------------------------------------------------    
        private void DeleteImage()
        {
            if (ImageFile != null)
            {
                int index = 0;
                for (int i = 0; i < PIECE_COUNT; i++)
                {                    
                    for (int j = 0; j < PIECE_COUNT; j++)
                    {
                        try
                        {                           
                            panel1.Controls.Remove(picCell[index]);
                            index++;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }        
        private void toolStripMenuItem9_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripMenuItem8_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Mô tả trò chơi:\n\nCó một tấm hình sẽ bị " +
                "chia nhỏ và xáo trộn vị trí, nhiệm vụ của bạn là phải ghép chúng lại vào khung để ra được hình giống với hình gốc." +
                "\n\nNếu muốn thêm hình khác vào thì hãy chọn Level trước khi thêm.", "About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            new Form2().ShowDialog();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            
        }               
    }
}
