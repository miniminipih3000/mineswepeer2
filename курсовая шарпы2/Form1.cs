using System;
using System.Drawing;
using System.Windows.Forms;

namespace saper
{
    public partial class Form1 : Form
    {
        const int n = 9;
        const int m = 10;
        bool[,] mina = new bool[n, n];
        int[,] chislo = new int[n, n];
        bool[,] otkr = new bool[n, n];
        bool[,] flag = new bool[n, n];
        Button[,] btn = new Button[n, n];
        int f = 0;
        bool game = false;
        bool first = true;
        public Form1()
        {
            InitializeComponent();
            makebtns();
            newgame();
        }
        void makebtns()
        {
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    Button b = new Button();
                    b.Size = new Size(50, 50);
                    b.Location = new Point(x * 50, y * 50);
                    b.Font = new Font("Arial", 12, FontStyle.Bold);
                    b.Tag = new Point(x, y);
                    b.MouseUp += klik;
                    b.FlatStyle = FlatStyle.Standard;

                    panel1.Controls.Add(b);
                    btn[x, y] = b;
                }
        }

        void newgame()
        {
            mina = new bool[n, n];
            chislo = new int[n, n];
            otkr = new bool[n, n];
            flag = new bool[n, n];
            f = 0;
            game = false;
            first = true;

            foreach (Button b in btn)
            {
                b.Text = "";
                b.BackColor = SystemColors.Control;
                b.ForeColor = Color.Black;
                b.Enabled = true;
            }

            label1.Text = $"Флагов: {f}/{m}";
        }
        void makemines(int sx, int sy)
        {
            Random r = new Random();
            int kol = 0;

            while (kol < m)
            {
                int x = r.Next(n);
                int y = r.Next(n);

                if (Math.Abs(x - sx) <= 1 && Math.Abs(y - sy) <= 1) continue;

                if (!mina[x, y])
                {
                    mina[x, y] = true;
                    kol++;
                }
            }
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    if (mina[x, y]) continue;
                    int cnt = 0;
                    for (int dy = -1; dy <= 1; dy++)
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = x + dx, ny = y + dy;
                            if (nx >= 0 && nx < n && ny >= 0 && ny < n && mina[nx, ny])
                                cnt++;
                        }
                    chislo[x, y] = cnt;
                }
        }
        void open(int x, int y)
        {
            if (otkr[x, y] || flag[x, y] || game) return;

            if (first)
            {
                makemines(x, y);
                first = false;
            }

            if (mina[x, y])
            {
                game = true;
                showmines();
                MessageBox.Show("Проиграл!", "Бум!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            otkr[x, y] = true;
            btn[x, y].Enabled = false;

            if (chislo[x, y] == 0)
            {
                for (int dy = -1; dy <= 1; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx >= 0 && nx < n && ny >= 0 && ny < n)
                            open(nx, ny);
                    }
            }

            updatebtn(x, y);
            checkwin();
        }

        void fl(int x, int y)
        {
            if (otkr[x, y] || game) return;

            if (flag[x, y])
            {
                flag[x, y] = false;
                f--;
                btn[x, y].Text = "";
                btn[x, y].ForeColor = Color.Black;
            }
            else
            {
                if (f >= m)
                {
                    MessageBox.Show("Флагов больше нельзя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                flag[x, y] = true;
                f++;
                btn[x, y].Text = "F";
                btn[x, y].ForeColor = Color.Red;
            }

            label1.Text = $"Флагов: {f}/{m}";
        }

        void updatebtn(int x, int y)
        {
            Button b = btn[x, y];
            if (otkr[x, y])
            {
                if (mina[x, y])
                {
                    b.Text = "*";
                    b.BackColor = Color.Red;
                    b.ForeColor = Color.White;
                }
                else if (chislo[x, y] > 0)
                {
                    b.Text = chislo[x, y].ToString();
                    setcolor(b, chislo[x, y]);
                }
                else
                {
                    b.Text = "";
                    b.BackColor = Color.LightGray;
                    b.ForeColor = Color.Black;
                }
            }
        }
        void setcolor(Button b, int num)
        {
            switch (num)
            {
                case 1: b.ForeColor = Color.Blue; break;
                case 2: b.ForeColor = Color.Green; break;
                case 3: b.ForeColor = Color.Red; break;
                case 4: b.ForeColor = Color.DarkBlue; break;
                case 5: b.ForeColor = Color.DarkRed; break;
                case 6: b.ForeColor = Color.Teal; break;
                case 7: b.ForeColor = Color.Black; break;
                case 8: b.ForeColor = Color.Gray; break;
            }
        }
        void showmines()
        {
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    if (mina[x, y])
                    {
                        btn[x, y].Text = "*";
                        btn[x, y].BackColor = Color.Red;
                        btn[x, y].ForeColor = Color.White;
                        btn[x, y].Enabled = false;
                    }
                }
        }

        void checkwin()
        {
            int opened = 0;
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    if (!mina[x, y] && otkr[x, y])
                        opened++;
                }

            if (opened == n * n - m)
            {
                game = true;
                MessageBox.Show("Победил!", "Ура!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void klik(object sender, MouseEventArgs e)
        {
            if (game) return;

            Button b = (Button)sender;
            Point p = (Point)b.Tag;
            int x = p.X;
            int y = p.Y;

            if (e.Button == MouseButtons.Left)
            {
                open(x, y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                fl(x, y);
            }
        }
        void button1_Click(object sender, EventArgs e)
        {
            newgame();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            newgame();
        }
    }
}