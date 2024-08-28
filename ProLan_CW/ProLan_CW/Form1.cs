using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProLan_CW
{
    public partial class Form1 : Form
    {
        public Form1()
        {            
            InitializeComponent();
            Initialize();
            this.Text = "Морской бой";            
        }



        public bool isPlaying;

        public int firstMove;

        public const int mapSize = 11;
        public const int cellSize = 50;
        public string alphabet = "АБВГДЕЖЗИК";

        int[] shipAll;
        public int ships;               
        public int enemyShips;

        public int selectedShip;
        public int selectedTilt;

        public int[,] myMap = new int[mapSize, mapSize];
        public int[,] enemyMap = new int[mapSize, mapSize];

        public Button[,] myButtons = new Button[mapSize, mapSize];
        public Button[,] enemyButtons = new Button[mapSize, mapSize];

        public Button[] buttonShips = new Button[4];
        public Button[] buttonTilt = new Button[2];

        public Button BT;
        public static Button buttonMyShips;
        public Button buttonEnemyShips;

        public Bot bot;
        


        public void Initialize()
        {
            Random r = new Random();

            isPlaying = false;
            firstMove = r.Next(0, 2);

            this.Controls.Clear();
            CreateMap();

            bot = new Bot(enemyMap, myMap, enemyButtons, myButtons);           

            shipAll = new int[4] { 4, 3, 2, 1 };
            ships = 10;
            enemyShips = 10;

            selectedShip = 3;
            selectedTilt = 0;            
        }



        public void CreateMap()
        {
            this.Width = cellSize * 28 + cellSize / 3;
            this.Height = cellSize * 14 - cellSize / 5;

            for (int i = 0; i < mapSize; i++) 
            { 
                for (int j = 0; j < mapSize; j++) 
                {
                    myMap[i, j] = 0;
                    enemyMap[i, j] = 0;

                    Button buttonMy = new Button();
                    buttonMy.Location = new Point(i * cellSize, j * cellSize);
                    buttonMy.Font = new Font(buttonMy.Font.Name, 15, FontStyle.Bold);
                    buttonMy.Size = new Size(cellSize, cellSize);
                    buttonMy.BackColor = Color.White;

                    Button buttonEnemy = new Button();
                    buttonEnemy.Location = new Point((i + 17) * cellSize, j * cellSize);
                    buttonEnemy.Font = buttonMy.Font;
                    buttonEnemy.Size = new Size(cellSize, cellSize);
                    buttonEnemy.BackColor = Color.White;

                    if (i == 0 || j == 0)
                    {
                        buttonMy.BackColor = Color.Blue;
                        buttonMy.ForeColor = Color.White;

                        buttonEnemy.BackColor = Color.Blue;
                        buttonEnemy.ForeColor = Color.White;

                        if (j != 0)
                        {
                            buttonMy.Text = j.ToString();
                            buttonEnemy.Text = j.ToString();
                        }

                        else if (i != 0)
                        {
                            buttonMy.Text = alphabet[i - 1].ToString();
                            buttonEnemy.Text = alphabet[i - 1].ToString();
                        }
                    }

                    else
                    {
                        buttonMy.Click += new EventHandler(ShipPlacement);
                        buttonEnemy.Click += new EventHandler(PlayerShoot);
                    }

                    myButtons[i, j] = buttonMy;
                    enemyButtons[i, j] = buttonEnemy;

                    this.Controls.Add(buttonMy);
                    this.Controls.Add(buttonEnemy);
                }               
            }

            Button buttonMyMap = new Button();
            buttonMyMap.Text = "Ваша карта";
            buttonMyMap.Location = new Point(0, cellSize * 11);
            buttonMyMap.Font = new Font(buttonMyMap.Font.Name, 15, FontStyle.Bold);
            buttonMyMap.Size = new Size(cellSize * 11, cellSize);
            this.Controls.Add(buttonMyMap);

            Button buttonEnemyMap = new Button();
            buttonEnemyMap.Text = "Карта противника";
            buttonEnemyMap.Location = new Point(cellSize * 17, cellSize * 11);
            buttonEnemyMap.Font = buttonMyMap.Font;
            buttonEnemyMap.Size = new Size(cellSize * 11, cellSize);
            this.Controls.Add(buttonEnemyMap);

            Button startButton = new Button();
            startButton.Text = "Старт";
            startButton.Location = new Point(cellSize * 12, cellSize * 12);
            startButton.Font = buttonMyMap.Font;
            startButton.Size = new Size(cellSize * 2, cellSize);
            startButton.BackColor = Color.Lime;
            startButton.Click += new EventHandler(Start);
            this.Controls.Add(startButton);

            Button stopButton = new Button();
            stopButton.Text = "Стоп";
            stopButton.Location = new Point(cellSize * 14, cellSize * 12);
            stopButton.Font = buttonMyMap.Font;
            stopButton.Size = new Size(cellSize * 2, cellSize);
            stopButton.BackColor = Color.Red;
            stopButton.Click += new EventHandler(Stop);
            this.Controls.Add(stopButton);

            Button buttonAuto = new Button();
            buttonAuto.Text = "Случайная расстановка";
            buttonAuto.Location = new Point(cellSize * 12, cellSize);
            buttonAuto.Font = new Font(buttonAuto.Font.Name, 12, FontStyle.Bold); ;
            buttonAuto.Size = new Size(cellSize * 4, cellSize);
            buttonAuto.BackColor = Color.Blue;
            buttonAuto.ForeColor = Color.White;
            buttonAuto.Click += new EventHandler(AutoShipPlacement);
            this.Controls.Add(buttonAuto);

            Button buttonShip3 = new Button();
            buttonShip3.Text = "1";
            buttonShip3.Location = new Point(cellSize * 12, cellSize * 3);
            buttonShip3.Font = buttonMyMap.Font;
            buttonShip3.Size = new Size(cellSize * 4, cellSize);
            buttonShip3.BackColor = Color.Red;
            buttonShip3.ForeColor = Color.Black;
            buttonShip3.Click += new EventHandler(ShipSelection);
            buttonShips[3] = buttonShip3;
            this.Controls.Add(buttonShip3);

            Button buttonShip2 = new Button();
            buttonShip2.Text = "2";
            buttonShip2.Location = new Point(cellSize * 12, cellSize * 4);
            buttonShip2.Font = buttonMyMap.Font;
            buttonShip2.Size = new Size(cellSize * 3, cellSize);
            buttonShip2.BackColor = Color.Blue;
            buttonShip2.ForeColor = Color.White;
            buttonShip2.Click += new EventHandler(ShipSelection);
            buttonShips[2] = buttonShip2;
            this.Controls.Add(buttonShip2);

            Button buttonShip1 = new Button();
            buttonShip1.Text = "3";
            buttonShip1.Location = new Point(cellSize * 12, cellSize * 5);
            buttonShip1.Font = buttonMyMap.Font;
            buttonShip1.Size = new Size(cellSize * 2, cellSize);
            buttonShip1.BackColor = Color.Blue;
            buttonShip1.ForeColor = Color.White;
            buttonShip1.Click += new EventHandler(ShipSelection);
            buttonShips[1] = buttonShip1;
            this.Controls.Add(buttonShip1);

            Button buttonShip0 = new Button();
            buttonShip0.Text = "4";
            buttonShip0.Location = new Point(cellSize * 12, cellSize * 6);
            buttonShip0.Font = buttonMyMap.Font;
            buttonShip0.Size = new Size(cellSize * 1, cellSize);
            buttonShip0.BackColor = Color.Blue;
            buttonShip0.ForeColor = Color.White;
            buttonShip0.Click += new EventHandler(ShipSelection);
            buttonShips[0] = buttonShip0;
            this.Controls.Add(buttonShip0);           

            Button buttonShip = new Button();
            buttonShip.Text = "Выберите корабль";
            buttonShip.Location = new Point(cellSize * 12, 0);
            buttonShip.Font = new Font(buttonShip.Font.Name, 13, FontStyle.Bold);
            buttonShip.Size = new Size(cellSize * 4, cellSize);
            buttonShip.BackColor = Color.Blue;
            buttonShip.ForeColor = Color.White;
            this.Controls.Add(buttonShip);

            Button buttonHorizontal = new Button();
            buttonHorizontal.Text = "Ставить горизонтально";
            buttonHorizontal.Location = new Point(cellSize * 12, cellSize * 2);
            buttonHorizontal.Font = new Font(buttonHorizontal.Font.Name, 8);
            buttonHorizontal.Size = new Size(cellSize * 2, cellSize);
            buttonHorizontal.BackColor = Color.Red;
            buttonHorizontal.ForeColor = Color.Black;
            buttonHorizontal.Click += new EventHandler(TiltSelection);
            buttonTilt[0] = buttonHorizontal;
            this.Controls.Add(buttonHorizontal);

            Button buttonVertical = new Button();
            buttonVertical.Text = "Ставить вертикально";
            buttonVertical.Location = new Point(cellSize * 14, cellSize * 2);
            buttonVertical.Font = new Font(buttonVertical.Font.Name, 8);
            buttonVertical.Size = new Size(cellSize * 2, cellSize);
            buttonVertical.BackColor = Color.Blue;
            buttonVertical.ForeColor = Color.White;
            buttonVertical.Click += new EventHandler(TiltSelection);
            buttonTilt[1] = buttonVertical;
            this.Controls.Add(buttonVertical);

            Button buttonText = new Button();
            buttonText.Text = "Расставьте все корабли (на кнопке указано оставшее количество кораблей данного типа)";
            buttonText.Location = new Point(cellSize * 12, cellSize * 8);
            buttonText.Font = new Font(buttonVertical.Font.Name, 13, FontStyle.Bold);
            buttonText.Size = new Size(cellSize * 4, cellSize * 3);
            BT = buttonText;
            this.Controls.Add(buttonText);            

            Button buttonCountMy = new Button();
            buttonCountMy.Text = "Количество ваших кораблей: 10";
            buttonCountMy.Location = new Point(0, cellSize * 12);
            buttonCountMy.Font = buttonMyMap.Font;
            buttonCountMy.Size = new Size(cellSize * 11, cellSize);
            buttonMyShips = buttonCountMy;
            this.Controls.Add(buttonCountMy);

            Button buttonCountEnemy = new Button();
            buttonCountEnemy.Text = "Количество кораблей противника: 10";
            buttonCountEnemy.Location = new Point(cellSize * 17, cellSize * 12);
            buttonCountEnemy.Font = buttonMyMap.Font;
            buttonCountEnemy.Size = new Size(cellSize * 11, cellSize * 1);
            buttonEnemyShips = buttonCountEnemy;
            this.Controls.Add(buttonCountEnemy);
        }



        public void Start(object sender, EventArgs e)
        {
            if (isPlaying)
                MessageBox.Show("Игра уже идёт", "Ошибка");

            else if (ships == 0)
            {
                isPlaying = true;
                BT.Text = "Идёт игра";

                if (firstMove == 1)
                    bot.BotShoot();
            }
            else
                MessageBox.Show("Расставьте все корабли", "Ошибка");
        }



        public void Stop(object sender, EventArgs e)
        {
            isPlaying = false;            
            Initialize();
        }



        public void AutoShipPlacement(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                for (int i = 1; i < mapSize; i++)
                {
                    for (int j = 1; j < mapSize; j++)
                    {
                        if (myMap[i, j] == 1)
                        {
                            myMap[i, j] = 0;
                            myButtons[i, j].BackColor = Color.White;
                        }
                    }
                }

                ships = 0;
                for (int n = 0; n < 4; n++)
                {
                    shipAll[n] = 0;
                    buttonShips[n].Text = "0";
                }

                bot.ConfigureShips(myMap, myButtons, true);
            }
        }



        public void ShipPlacement(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;

            int cordX = pressedButton.Location.X / cellSize;
            int cordY = pressedButton.Location.Y / cellSize;
            int i, j;

            bool border;
            bool ship = true;            

            if (selectedTilt == 0)
            {
                i = cordX;
                j = cordX;
            }
            else
            {
                i = cordY;
                j = cordY;
            }

            if (!isPlaying)
            {
                if (myMap[cordX, cordY] == 0)
                {                    
                    if (shipAll[selectedShip] > 0)
                    {
                        if (selectedTilt == 0)
                            border = CheckBorders(cordX + selectedShip, cordY);
                        else
                            border = CheckBorders(cordX, cordY + selectedShip);

                        if (!border)
                            MessageBox.Show("Корабль не может выходить за пределы игровой карты", "Ошибка");

                        for (; i < j + selectedShip + 1; i++)
                        {
                            if (selectedTilt == 0)
                                cordX = i;
                            else
                                cordY = i;

                            if (CheckShips(cordX, cordY, myMap))
                            {
                                ship = false;
                                MessageBox.Show("Корабли не могут касаться друг друга", "Ошибка");
                                break;
                            }
                        }

                        if (border && ship)
                        {
                            ships--;
                            shipAll[selectedShip]--;
                            buttonShips[selectedShip].Text = shipAll[selectedShip].ToString();

                            i = j;
                            for (; i < j + selectedShip + 1; i++)
                            {
                                if (selectedTilt == 0)
                                    cordX = i;
                                else
                                    cordY = i;

                                myMap[cordX, cordY] = 1;
                                myButtons[cordX, cordY].BackColor = Color.Red;
                            }
                        }
                    }                    
                }

                else
                {
                    RemoveShip(cordX, cordY, myMap, myButtons);
                }
            }
        }



        public void ShipSelection(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;

            for (int i = 0; i < 4; i++)
            {
                if (pressedButton == buttonShips[i]) 
                { 
                    selectedShip = i;
                    buttonShips[i].BackColor = Color.Red;
                    buttonShips[i].ForeColor = Color.Black;
                }

                else
                {
                    buttonShips[i].BackColor = Color.Blue;
                    buttonShips[i].ForeColor = Color.White;
                }
            }
        }



        public void TiltSelection(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;

            for (int i = 0; i < 2; i++)
            {
                if (pressedButton == buttonTilt[i])
                {
                    selectedTilt = i;
                    buttonTilt[i].BackColor = Color.Red;
                    buttonTilt[i].ForeColor = Color.Black;
                }

                else
                {
                    buttonTilt[i].BackColor = Color.Blue;
                    buttonTilt[i].ForeColor = Color.White;
                }
            }
        }



        public static bool CheckBorders(int i, int j) 
        {
            if (i < mapSize && j < mapSize)
                return true;
            else                         
                return false;
            
        }



        public static bool CheckShips(int i, int j, int[,] map)
        {
            for (int i1 = i - 1; i1 < i + 2; i1++)
            {
                for(int j1 = j - 1; j1 < j + 2; j1++)
                {
                    if (i1 < mapSize && j1 < mapSize && map[i1, j1] == 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }



        public int[,] RemoveShip(int i, int j, int[,] map, Button[,] buttons)
        {
            int sum = -1;

            if (map[i - 1, j] == 1) 
            {
                for (int n = i - 1; 0 < n; n--)
                    if (map[n, j] == 0)
                        break;
                    else
                        i = n;
            }   

            else if (map[i, j - 1] == 1) 
            {
                for (int n = j - 1; 0 < n; n--)
                    if (map[i, n] == 0)
                        break;
                    else
                        j = n;
            }

            if (i + 1 < mapSize && map[i + 1, j] == 1)
            {
                for (int n = i; n < mapSize; n++)
                    if (map[n, j] == 1)
                    {
                        map[n, j] = 0;
                        buttons[n, j].BackColor = Color.White;
                        sum++;
                    }
                    else
                        break;
            }

            else if (j + 1 < mapSize && map[i, j + 1] == 1)
            {
                for (int n = j; n < mapSize; n++)
                    if (map[i, n] == 1)
                    {
                        map[i, n] = 0;
                        buttons[i, n].BackColor = Color.White;
                        sum++;
                    }
                    else
                        break;
            }

            else 
            {
                map[i, j] = 0;
                buttons[i, j].BackColor = Color.White;
                sum++;
            }

            ships++;
            shipAll[sum]++;
            buttonShips[sum].Text = shipAll[sum].ToString();

            return map;
        }



        public static bool DestroyedShip(int i, int j, int past_i, int past_j, int[,] map, Button[,] buttons)
        {
            bool destroyedShip = true;

            for (int i1 = i - 1; i1 < i + 2; i1++)
            {
                for (int j1 = j - 1; j1 < j + 2; j1++)
                {
                    if ((i1 != i || j1 != j) && (i1 != past_i || j1 != past_j) && i1 < mapSize && j1 < mapSize && map[i1, j1] == 2)
                    {
                        if(!DestroyedShip(i1, j1, i, j, map, buttons))
                            return false;
                    }                       

                    else if (i1 < mapSize && j1 < mapSize && map[i1, j1] == 1)
                        return false;                    
                }
            }

            return destroyedShip;
        }



        public static void MarkDestroyedShip(int i, int j, int past_i, int past_j, int[,] map, Button[,] buttons)
        {
            for (int i1 = i - 1; i1 < i + 2; i1++)
            {
                for (int j1 = j - 1; j1 < j + 2; j1++)
                {
                    if ((i1 != i || j1 != j) && (i1 != past_i || j1 != past_j) && i1 < mapSize && j1 < mapSize && map[i1, j1] == 2)
                        MarkDestroyedShip(i1, j1, i, j, map, buttons);

                    else if (0 < i1 && 0 < j1 && i1 < mapSize && j1 < mapSize && map[i1, j1] != 2)
                    {
                        buttons[i1, j1].BackColor = Color.Gray;
                    }
                }
            }
        }        



        public bool Win(int myShips, int enemyShips)
        {
            if (enemyShips == 0)
            {
                BT.Text = "Игра окончена";
                MessageBox.Show("Вы победили!", "Игра окончена");
                return true;
            }

            else if (myShips == 0)
            {
                BT.Text = "Игра окончена";
                MessageBox.Show("Вы проиграли!", "Игра окончена");
                return true;
            }

            return false;
        }



        public bool Shoot(int[,] map, Button pressedButton)
        {
            bool hit = false;

            if (isPlaying)
            {
                int cordX = (pressedButton.Location.X - cellSize * 17) / cellSize;
                int cordY = pressedButton.Location.Y / cellSize;
                

                if (map[cordX, cordY] == 1)
                {
                    hit = true;

                    pressedButton.BackColor = Color.Red;
                    pressedButton.Text = "X";
                    enemyMap[cordX, cordY] = 2;

                    if (DestroyedShip(cordX, cordY, cordX, cordY, enemyMap, enemyButtons))
                    {
                        enemyShips--;                        
                        buttonEnemyShips.Text = "Количество кораблей противника: " + enemyShips.ToString();
                        MarkDestroyedShip(cordX, cordY, cordX, cordY, enemyMap, enemyButtons);
                    }
                }

                else if (enemyButtons[cordX, cordY].Text != "X")
                {
                    hit = false;

                    pressedButton.BackColor = Color.Gray;
                }
            }

            return hit;
        }
        


        public void PlayerShoot(object sender, EventArgs e)
        {
            Button pressedButton = sender as Button;

            if (isPlaying)
            {
                bool playerShoot = Shoot(enemyMap, pressedButton);
                
                if (!playerShoot)                    
                    bot.BotShoot();

                if (Win(bot.enemyShips, enemyShips))                                    
                    Initialize();                                
            }
        }
    }
}
