using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphCSharp
{
    public partial class Form1 : Form
    {
        public class Vertex //Класс вершин 
        {
            public Label labeln;
            public Vertex(Label labeln)
            {
                this.labeln = labeln;
            }
        };

        public class Edge //Класс ребер
        {
            public Label v1, v2;
            public Edge(Label v1, Label v2)
            {
                this.v1 = v1;
                this.v2 = v2;
            }
        };

        int select = 0;
        List<Vertex> V;
        List<Edge> E;
        List<TextBox> tbList = new List<TextBox>();
        Queue<TextBox> tbQueue = new Queue<TextBox>();
        Queue<Label> queue;
        Stack<Label> stack;
        Label secondLabel;
        Bitmap line = new Bitmap(720, 480);
        TextBox[] tbArr;
        int[] peekStatus;
        int[,] matrixSm;
        String[,] matrixIn;
        bool move = false;
        TextBox tempTb;
        Point prev, tempTbLocation,prevTbLocation;
        public Form1()
        {
            InitializeComponent();
            V = new List<Vertex>();
            E = new List<Edge>();
        }
        //Устанавливаем вершины
        public void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)//Set Vertex
            {   
                if(V.Count!=0)
                {
                redrawgraph();
                }
                Label labeln = new Label();
                labeln.MouseClick += new MouseEventHandler(this.labeln_rClick);
                labeln.MouseDown+=new MouseEventHandler(this.labeln_down);
                labeln.MouseUp += new MouseEventHandler(this.labeln_up);
                labeln.MouseMove += new MouseEventHandler(this.labeln_move);
                int cursorx = 0, cursory = 0;
                cursorx = e.X;
                cursory = e.Y;
                //Установка лейбла и его параметров
                labeln.Text = Convert.ToString(V.Count);
                labeln.Location = new Point(cursorx - 15, cursory - 15);
                Controls.Add(labeln);
                labeln.BackColor = Color.Red;
                labeln.Size = new Size(30, 30);
                labeln.TextAlign = ContentAlignment.MiddleCenter;
                labeln.BringToFront();
                //Установка точки
                V.Add(new Vertex(labeln));
                MatrixSm();
                MatrixIn();
            }
        }
        private void labeln_down(object sender, MouseEventArgs e)   //если зажали кнопку то перемещение включилось
        {
            Label labeln = sender as Label;
            int l = 0;
            tbArr = new TextBox[tbList.Count];
            if (e.Button == MouseButtons.Left && checkBox1.Checked == true)
            {  
                for (int i = 0; i < V.Count; i++) //Ищем тот лейбл который собрались двигать
                {
                    if (labeln.Location == V[i].labeln.Location)
                    {
                        prev = V[i].labeln.Location; //Запоминаем его
                    }
                }
                for(int i=0;i<V.Count;i++) //ищем текстблок который нужно перетащить
                {
                    if (matrixSm[Convert.ToInt16(labeln.Text),i]!=0)
                    {   
                        for(int j=0;j<tbList.Count;j++)
                        { tempTbLocation = new Point((labeln.Location.X + 15 + V[i].labeln.Location.X) / 2, (labeln.Location.Y + 15 + V[i].labeln.Location.Y) / 2);
                            if (tbList[j].Location==tempTbLocation)
                            {
                                tbArr[l] = tbList[j];
                                l++;
                            }
                        }
                    }
                }
            }
        }

        private void labeln_up(object sender,MouseEventArgs e) //если отжали кнопку то перемещение выключилось
        {
            Label labeln = sender as Label;
            if (e.Button == MouseButtons.Left && checkBox1.Checked == true)
            {
                for (int i = 0; i < V.Count; i++) //Находим лейбл который двигаем и устанавливаем ему в списке новую локацию
                {
                    if (prev == V[i].labeln.Location)
                    {
                        V[i].labeln = labeln; //ставим ему новую локацию
                    }
                }
            }
        }

        private void labeln_move(object sender, MouseEventArgs e)
        {
            Label labeln = sender as Label;
            if (e.Button == MouseButtons.Left && checkBox1.Checked == true)
            {   
                Graphics graphic = Graphics.FromImage(line);
                labeln.Location = new Point((Cursor.Position.X - this.Location.X) - 25, (Cursor.Position.Y - this.Location.Y) - 45);
                for (int i=0;i<V.Count;i++)
                {
                    if (matrixSm[Convert.ToInt16(labeln.Text), i] != 0)
                    {   
                        Pen penBlack = new Pen(Color.Black, 3);
                        graphic.Clear(Color.White);
                        redrawgraph();
                        pictureBox1.Image = line;
                    }
                }
                for (int j = 0; j < tbList.Count; j++)
                {
                    for (int i = 0; i < V.Count; i++)
                    {
                        if (matrixSm[Convert.ToInt16(labeln.Text), i] != 0)
                        {
                            prevTbLocation = new Point((prev.X + 15 + V[i].labeln.Location.X) / 2, (prev.Y + 15 + V[i].labeln.Location.Y) / 2);
                            for (int k = 0; k < tbArr.Count(); k++)
                            {
                                tempTb = tbArr[k];
                                if (tempTb == null)
                                {
                                    continue;
                                }
                                if (tbList[j].Location == tempTb.Location && tempTb.Location == prevTbLocation)
                                {
                                    tbList[j].Location = new Point((labeln.Location.X + 15 + V[i].labeln.Location.X) / 2, (labeln.Location.Y + 15 + V[i].labeln.Location.Y) / 2);
                                }
                            }
                        }
                    }
                }
                prev = labeln.Location;
            }
        }

        private void labeln_rClick(object sender, MouseEventArgs e)//Выбираем и соеденяем вершины
        {
            Graphics graphic = Graphics.FromImage(line);
            TextBox tb = new TextBox();
            Label labeln = sender as Label;
            if (e.Button == MouseButtons.Left&& checkBox1.Checked==false)
            {
                if (V.Count != 0||move)
                {
                    redrawgraph();
                }
                select++;
                labeln.BackColor = Color.Green;
                if (select == 2)//connect
                {
                    if (labeln == secondLabel)
                    {
                        secondLabel.BackColor = Color.Red;
                        select = 0;
                        return;
                    }
                    //Устанавливаем текстбоксы на ребра 
                    tb.Location = new Point((secondLabel.Location.X + 15 + labeln.Location.X) / 2, (secondLabel.Location.Y + 15 + labeln.Location.Y) / 2);
                    Controls.Add(tb);
                    tb.Size = new Size(15, 10);
                    tb.Text = "1";
                    tb.BringToFront();
                    tbList.Add(tb);
                    //Устанавливаем ребра
                    Pen pen = new Pen(Color.Black, 3);
                    graphic.DrawLine(pen, labeln.Location.X + 15, labeln.Location.Y + 15, secondLabel.Location.X + 15, secondLabel.Location.Y + 15);
                    pictureBox1.Image = line;
                    //делаем лейблы обратно красными
                    labeln.BackColor = Color.Red;
                    secondLabel.BackColor = Color.Red;
                    select = 0;
                    E.Add(new Edge(secondLabel, labeln));
                    E.Add(new Edge(labeln, secondLabel));
                    secondLabel = null;
                    MatrixSm();
                    MatrixIn();
                    return;
                }
                //запоминаем какой лейбл был выбран первым
                secondLabel = labeln;
            }


            if (e.Button == MouseButtons.Right)//Delete Verte
            {
                if (secondLabel == labeln)
                {
                    secondLabel = null;
                    select = 0;
                }
                if (E.Count != 0)
                {
                    redrawgraph();
                }
                Point point1, point2, tbLocation;
                int deletedLabel = -1;
                for (int i = 0; i < V.Count; i++)
                {
                    if (labeln.Location == V[i].labeln.Location)
                    {
                        for (int j = 0; j < E.Count; j++)
                        {
                            if (E[j].v1 == V[i].labeln || E[j].v2 == V[i].labeln) //Удаление линий
                            {
                                point1 = E[j].v1.Location;
                                point2 = E[j].v2.Location;
                                Pen pen = new Pen(Color.White, 3);
                                graphic.DrawLine(pen, point1.X + 15, point1.Y + 15, point2.X + 15, point2.Y + 15);
                                graphic.DrawLine(pen, point2.X + 15, point2.Y + 15, point1.X + 15, point1.Y + 15);
                                pictureBox1.Image = line;
                                tbLocation = new Point((point1.X + 15 + point2.X) / 2, (point1.Y + 15 + point2.Y) / 2);
                                for (int k = 0; k < tbList.Count; k++) //Нахождение и удаление текстбокса
                                {
                                    if (tbList[k].Location == tbLocation)
                                    {
                                        tbList[k].Dispose();
                                        tbList.RemoveAt(k);
                                    }
                                }
                                E.RemoveAt(j);
                                E.RemoveAt(j);
                                    j--;
                            }
                        }
                        deletedLabel = V.IndexOf(V[i]);
                        V.RemoveAt(i);
                        labeln.Dispose();
                    }
                }
                if (deletedLabel > -1) //Переименовываем елементы если удаляем один
                {
                    for (int i = deletedLabel; i < V.Count; i++)
                    {
                        if (V.IndexOf(V[i]) != V.Count)
                        V[i].labeln.Text = Convert.ToString(V.IndexOf(V[i]));
                    }
                }
                MatrixSm();
                MatrixIn();
            }
        }

        private void button1_Click(object sender, EventArgs e) //обновление матриц
        {
            MatrixSm();
            MatrixIn();
        }

        private void MatrixSm() //матрица смежности
        {
            if (V.Count == 0)
            {
                dataGridView1.RowCount = 1;
                dataGridView1.ColumnCount = 1;
                dataGridView1.RowHeadersWidth = 55;
                return;
            }
            Point tbLocation;
            matrixSm = new int[V.Count, V.Count];
            dataGridView1.RowCount = V.Count;
            dataGridView1.ColumnCount = V.Count;
            dataGridView1.RowHeadersWidth = 55;
            //Заполняем масив
            for (int i = 0; i < E.Count; i++)
            {
                tbLocation = new Point((E[i].v1.Location.X + 15 + E[i].v2.Location.X) / 2, (E[i].v1.Location.Y + 15 + E[i].v2.Location.Y) / 2);
                for (int j = 0; j < tbList.Count; j++) //Находим текстблок отвечающий за ребро
                {
                    if (tbList[j].Location == tbLocation)
                    {
                        matrixSm[Convert.ToInt16(E[i].v1.Text), Convert.ToInt16(E[i].v2.Text)] = Convert.ToInt16(tbList[j].Text);
                    }
                }
            }
            for (int i = 0; i < V.Count; i++) //Создаем матрицу
            {
                dataGridView1.Columns[i].Width = 30;
                dataGridView1.Rows[i].Height = 30;
                dataGridView1.Columns[i].Name = "V" + Convert.ToString(i);
                dataGridView1.Rows[i].HeaderCell.Value = "V" + Convert.ToString(i);
            }
            for (int i = 0; i < V.Count; i++) //Заполняем значениями из масива
            {
                for (int j = 0; j < V.Count; j++)
                {
                    dataGridView1[i, j].Value = matrixSm[i, j];
                }
            }
        }

        private void MatrixIn()//Матрица инцидентности
        {
            if (V.Count == 0 || tbList.Count == 0)
            {
                dataGridView2.RowCount = 1;
                dataGridView2.ColumnCount = 1;
                dataGridView2.RowHeadersWidth = 55;
                return;
            }
            dataGridView2.RowCount = V.Count;
            dataGridView2.ColumnCount = tbList.Count;
            dataGridView2.RowHeadersWidth = 55;
            Point tbLocation;
            matrixIn = new String[V.Count, E.Count / 2];
            for (int i = 0; i < E.Count; i++)
            {
                tbLocation = new Point((E[i].v1.Location.X + 15 + E[i].v2.Location.X) / 2, (E[i].v1.Location.Y + 15 + E[i].v2.Location.Y) / 2);
                for (int j = 0; j < tbList.Count; j++) //Находим текстблок отвечающий за ребро
                {
                    if (tbList[j].Location == tbLocation)
                    {
                        matrixIn[Convert.ToInt16(E[i].v1.Text), j] = tbList[j].Text;
                    }
                }
            }
            for (int i = 0; i < V.Count; i++) //Создаем рядки
            {
                dataGridView2.Rows[i].Height = 30;
                dataGridView2.Rows[i].HeaderCell.Value = "V" + Convert.ToString(i);
            }
            for (int i = 0; i < tbList.Count; i++)
            {
                dataGridView2.Columns[i].Width = 30;
                dataGridView2.Columns[i].Name = "E" + i;
            }
            for (int i = 0; i < V.Count; i++) //Заполняем значениями из масива
            {
                for (int j = 0; j < tbList.Count; j++)
                {
                    dataGridView2[j, i].Value = matrixIn[i, j];
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) //Обход в шырину
        {
            if (secondLabel == null)    //Смотрим какая вершина выбрана, если никакая - то строим от 0
                secondLabel = V[0].labeln;
            listBox1.Items.Clear();
            string text = " ";
            queue = new Queue<Label>();
            peekStatus = new int[V.Count];
            for (int i = 0; i < V.Count; i++)//отмечаем все вершины как непосещенные
                peekStatus[i] = 1;
            Label current = secondLabel; //Ведем отсчет от выбраной вершины
            queue.Enqueue(current);
            peekStatus[Convert.ToInt16(current.Text)] = 2; //1 - не отмеченая,2 - отмеченная но не посещенная, 3 - посещенная

            while (queue.Count != 0)
            {
                current = queue.Dequeue();
                if (peekStatus[Convert.ToInt16(current.Text)] == 2)
                {
                    for (int j = 0; j < V.Count; j++)
                    {
                        if (matrixSm[Convert.ToInt16(current.Text), j] != 0)
                        {
                            if (peekStatus[j] != 3)
                            {
                                queue.Enqueue(V[j].labeln);
                                peekStatus[j] = 2;
                            }
                        }
                    }
                    peekStatus[Convert.ToInt16(current.Text)] = 3;
                    text += current.Text + "->";
                    listBox1.Items.Add(text);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)//Обход в глубину
        {
            if (secondLabel == null) //Смотрим какая вершина выбрана, если никакая - то строим от 0
            secondLabel = V[0].labeln;
            listBox1.Items.Clear();
            string text = " ";
            stack = new Stack<Label>(V.Count);
            peekStatus = new int[V.Count];
            for (int i = 0; i < V.Count; i++)//отмечаем все вершины как непосещенные
            peekStatus[i] = 1;
            Label current = secondLabel;
            stack.Push(current);
            peekStatus[Convert.ToInt16(current.Text)] = 2; //1 - не отмеченая,2 - отмеченная но не посещенная, 3 - посещенная
            while (stack.Count != 0)
            {
                current = stack.Pop();
                if (peekStatus[Convert.ToInt16(current.Text)] == 2)
                {
                    for (int j = 0; j < V.Count; j++)
                    {
                        if (matrixSm[Convert.ToInt16(current.Text), j] != 0)
                        {
                            if (peekStatus[j] != 3)
                            {
                                stack.Push(V[j].labeln);
                                peekStatus[j] = 2;
                            }
                        }
                    }
                    peekStatus[Convert.ToInt16(current.Text)] = 3;
                    text += current.Text + "->";
                    listBox1.Items.Add(text);
                }
            }
        }
        
        private void button5_Click(object sender, EventArgs e)//удаление графа
        {
            if (V.Count == 0 && E.Count == 0)
            {
                return;
            }
            Graphics graphic = Graphics.FromImage(line);
            graphic.Clear(Color.White);
            pictureBox1.Image = line;
            for (int i = 0; i < V.Count; i++)
            {
                V[i].labeln.Dispose();
            }
            V.Clear();
            for (int i = 0; i < E.Count; i++)
            {
                E[i].v1.Dispose();
                E[i].v2.Dispose();
            }
            E.Clear();
            for (int i = 0; i < tbList.Count; i++)
            {
                tbList[i].Dispose();
            }
            tbList.Clear();
            select = 0;
            secondLabel = null;
            listBox1.Items.Clear();
            MatrixIn();
            MatrixSm();
        }

        private void button4_Click(object sender, EventArgs e) //Алгоритм дейкстры
        {
            if (secondLabel == null||V.Count==0)
            {
                return;
            }
            int min = int.MaxValue;
            int[] path = new int[V.Count];
            int pathvalue = 0;
            bool[] dontneedToCheck = new bool[V.Count];
            listBox1.Items.Clear();
            string text = " ";
            peekStatus = new int[V.Count];
            for (int i = 0; i < V.Count; i++)//отмечаем все вершины как непосещенные
            {
                dontneedToCheck[i] = false;
                peekStatus[i] = 1;
                path[i] = int.MaxValue;//Ставим путь всем точкам - бесконечность
            }
            Label current = V[0].labeln;//начинаем от нулевой вершины
            peekStatus[Convert.ToInt16(current.Text)] = 2; //1 - не отмеченая,2 - отмеченная но не посещенная, 3 - посещенная
            path[0] = 0;
            while (current != secondLabel)
            {
                min = int.MaxValue;
                for (int i = 0; i < V.Count; i++) //находим непосещенную вершину с наименшим путем
                {
                    if (min > path[i] && dontneedToCheck[i] != true)
                    {
                        current = V[i].labeln;
                        min = path[i];
                    }
                }
                dontneedToCheck[Convert.ToInt16(current.Text)] = true;
                if (peekStatus[Convert.ToInt16(current.Text)] == 2)
                {
                    for (int i = 0; i < V.Count; i++)
                    {
                        if (matrixSm[Convert.ToInt16(current.Text), i] != 0 && dontneedToCheck[i] != true)
                        {
                            if (matrixSm[Convert.ToInt16(current.Text), i] > path[i])
                            {
                                continue;
                            }
                            if (path[i] == int.MaxValue)
                            {
                                path[i] = path[Convert.ToInt16(current.Text)] + matrixSm[Convert.ToInt16(current.Text), i];
                                peekStatus[i] = 2;
                                continue;
                            }
                            if (matrixSm[Convert.ToInt16(current.Text), i] + path[Convert.ToInt16(current.Text)] < path[i])
                            {
                                path[i] = path[Convert.ToInt16(current.Text)] + matrixSm[Convert.ToInt16(current.Text), i];
                                peekStatus[i] = 2;
                                continue;
                            }
                        }
                    }
                    if (current == secondLabel)
                    {
                        break;
                    }
                }
            }
            pathvalue = path[Convert.ToInt16(current.Text)];
            current = secondLabel;
            string[] elem = new string[V.Count];
            int temp = 0, k = 0 ;
            while(current!=V[0].labeln)
            {
                for (int j=0;j< V.Count; j++)
                {
                    if (matrixSm[Convert.ToInt16(current.Text), j] != 0)
                    {
                        temp = pathvalue - matrixSm[Convert.ToInt16(current.Text), j];
                        if (temp==path[j])
                        {
                            elem[k] = current.Text;
                            k++;
                            current = V[j].labeln;
                            pathvalue = temp;
                        }
                    }
                }
            }
             text += "0";
             for(;k!=-1;k--)
             {
                 text += elem[k]+ "->";
             }
            listBox1.Items.Add(text);
        }

        private void button6_Click(object sender, EventArgs e) //Алгоритм прима
        {
            if (V.Count == 0)
            {
                return;
            }
            Graphics graphic = Graphics.FromImage(line);
            bool[] dontneedToCheck = new bool[V.Count];
            int min = int.MaxValue;
            Label current = V[0].labeln, secondLbl=V[0].labeln ;
            for(int i=0;i<V.Count;i++)
            {
                dontneedToCheck[i] = false;
            }
            dontneedToCheck[0] = true;
            int k = V.Count;
            while (k != 0)
            {   
                for (int i = 0; i < V.Count; i++) //находим непосещенную вершину, граничащую с провереными, с наименшим путем
                {   
                    if(dontneedToCheck[i]==false)
                    {
                        continue;
                    }
                    for (int j = 0; j < V.Count; j++)
                    {
                        if (min > matrixSm[i, j] && dontneedToCheck[j] == false && matrixSm[i, j] != 0)
                        {
                         secondLbl = V[i].labeln;
                         current = V[j].labeln;
                         min = matrixSm[i, j];
                        }
                    }  
                }
                Pen pen = new Pen(Color.Green, 3);
                graphic.DrawLine(pen, current.Location.X + 15, current.Location.Y + 15, secondLbl.Location.X + 15, secondLbl.Location.Y + 15);
                pictureBox1.Image = line;
                min = int.MaxValue;
                dontneedToCheck[Convert.ToInt16(current.Text)] = true;
                k--;
            }
        }

        private void redrawgraph()
        {   
            if (E.Count ==0)
            {
                return;
            }
            Point point1, point2;
            Graphics graphic = Graphics.FromImage(line);
            for (int i = 0; i < V.Count; i++)
            {
                for (int j = 0; j < E.Count; j++)
                {
                    //перерисовка линий
                    point1 = E[j].v1.Location;
                    point2 = E[j].v2.Location;
                    Pen pen = new Pen(Color.Black, 3);
                    graphic.DrawLine(pen, point1.X + 15, point1.Y + 15, point2.X + 15, point2.Y + 15);
                    graphic.DrawLine(pen, point2.X + 15, point2.Y + 15, point1.X + 15, point1.Y + 15);
                    pictureBox1.Image = line;
                }
            }
        } 
    }
}
