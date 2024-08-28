using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProLan_CW
{
    public class Bot
    {
        public int cordShipX;
        public int cordShipY;

        public int left_right;
        public int up_down;

        public bool wasHit = false;

        public int[,] myMap = new int[Form1.mapSize, Form1.mapSize];
        public int[,] enemyMap = new int[Form1.mapSize, Form1.mapSize];

        public Button[,] myButtons = new Button[Form1.mapSize, Form1.mapSize];
        public Button[,] enemyButtons = new Button[Form1.mapSize, Form1.mapSize];

        public int enemyShips;

        public Bot(int[,] myMap, int[,] enemyMap, Button[,] myButtons, Button[,] enemyButtons)
        {
            this.myMap = myMap;
            this.enemyMap = enemyMap;
            this.myButtons = myButtons;
            this.enemyButtons = enemyButtons;

            ConfigureShips(myMap, myButtons, false);

            enemyShips = 10;
        }



        public int[,] ConfigureShips(int[,] map, Button[,] buttons, bool isPlayer)
        {
            Random r = new Random();                  

            int[] shipAll = new int[4] { 4, 3, 2, 1 };

            int i, j;                      

            for (int n = 3; n >= 0;)
            {
                if (shipAll[n] > 0)
                {
                    int cordX = r.Next(1, Form1.mapSize);
                    int cordY = r.Next(1, Form1.mapSize);
                    int selectedTilt = r.Next(0, 2);

                    bool border;
                    bool ship = true;

                    if (map[cordX, cordY] == 0)
                    {                        
                        if (selectedTilt == 0)
                            border = Form1.CheckBorders(cordX + n, cordY);
                        else
                            border = Form1.CheckBorders(cordX, cordY + n);

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

                        for (; i < j + n + 1; i++)
                        {
                            if (selectedTilt == 0)
                                cordX = i;
                            else
                                cordY = i;

                            if (Form1.CheckShips(cordX, cordY, map))
                            {
                                ship = false;
                                break;
                            }
                        }

                        if (border && ship)
                        {
                            shipAll[n]--;

                            i = j;
                            for (; i < j + n + 1; i++)
                            {
                                if (selectedTilt == 0)
                                    cordX = i;
                                else
                                    cordY = i;

                                map[cordX, cordY] = 1;
                                if(isPlayer)
                                    buttons[cordX, cordY].BackColor = Color.Red;
                            }
                        }                        
                    }
                }

                else
                    n--;
            }

            return map;
        }       



        public bool HitShoot(int i, int j)
        {
            if (0 < i && 0 < j && i < Form1.mapSize && j < Form1.mapSize)
            {
                if (enemyMap[i, j] == 0)
                {
                    enemyButtons[i, j].BackColor = Color.Gray;
                    return false;
                }

                else
                {
                    enemyMap[i, j] = 2;
                    enemyButtons[i, j].Text = "X";
                    return true;
                }

            }

            else
                return false;
        }



        public void AfterShipDestroyed(int i, int j, bool hit)
        {
            wasHit = false;

            enemyShips--;
            Form1.buttonMyShips.Text = "Количество ваших кораблей: " + enemyShips.ToString();
            Form1.MarkDestroyedShip(i, j, i, j, enemyMap, enemyButtons);

            if (enemyShips == 0)
            {
                hit = false;
            }
        }

        

        public bool CheckCell(int i, int j)
        {
            if (!(enemyButtons[i, j].BackColor == Color.White || enemyMap[i, j] == 1))
                return false;

            if (i + 1 < Form1.mapSize && (enemyButtons[i + 1, j].BackColor == Color.White || enemyMap[i + 1, j] == 1))
                return true;

            else if (enemyButtons[i - 1, j].BackColor == Color.White || enemyMap[i - 1, j] == 1)
                return true;

            else if (j + 1 < Form1.mapSize && (enemyButtons[i, j + 1].BackColor == Color.White || enemyMap[i, j + 1] == 1))
                return true;

            else if (enemyButtons[i, j - 1].BackColor == Color.White || enemyMap[i, j - 1] == 1)
                return true;

            return false;
        }



        public bool BotShoot()
        {
            Random r = new Random();

            bool hit = false;

            int cordX = r.Next(1, Form1.mapSize);
            int cordY = r.Next(1, Form1.mapSize);

            int k = 0;

            while (k < 100)
            {
                cordX = r.Next(1, Form1.mapSize);
                cordY = r.Next(1, Form1.mapSize);

                if (CheckCell(cordX, cordY))
                    break;

                k++;
            }            

            if (k >= 100)
            {
                for (int i = 1; i < Form1.mapSize; i++)
                {
                    for (int j = 1; j < Form1.mapSize; j++)
                    {
                        if (enemyMap[i, j] == 1 || enemyButtons[i, j].BackColor == Color.White)
                        {
                            cordX = i;
                            cordY = j;
                            break;
                        }
                    }
                }
            }

            if (wasHit)
            {
                bool right = false;
                int n = -1;

                if (left_right == 0 && up_down == 0)
                    while (!right)
                    {
                        n = r.Next(0, 4);

                        if (n == 0)
                            right = (cordShipX + 1 < Form1.mapSize && enemyButtons[cordShipX + 1, cordShipY].BackColor != Color.Gray &&
                                                                      enemyMap[cordShipX + 1, cordShipY] != 2);

                        else if (n == 1)
                            right = (0 < cordShipX - 1 && enemyButtons[cordShipX - 1, cordShipY].BackColor != Color.Gray &&
                                                          enemyMap[cordShipX - 1, cordShipY] != 2);

                        else if (n == 2)
                            right = (cordShipY + 1 < Form1.mapSize && enemyButtons[cordShipX, cordShipY + 1].BackColor != Color.Gray &&
                                                                      enemyMap[cordShipX, cordShipY + 1] != 2);

                        else
                            right = (0 < cordShipY - 1 && enemyButtons[cordShipX, cordShipY - 1].BackColor != Color.Gray &&
                                                           enemyMap[cordShipX, cordShipY - 1] != 2);
                    }

                if (n == 0 || left_right < 0)
                {
                    for (int i = cordShipX + 1; i < Form1.mapSize; i++)
                        if (HitShoot(i, cordShipY))
                            left_right++;
                        else
                            break;
                }

                else if (n == 1 || left_right > 0)
                {
                    for (int i = cordShipX - 1; 0 < i; i--)
                        if (HitShoot(i, cordShipY))
                            left_right--;
                        else
                            break;
                }

                else if (n == 2 || up_down < 0)
                {
                    for (int i = cordShipY + 1; i < Form1.mapSize; i++)
                        if (HitShoot(cordShipX, i))
                            up_down++;
                        else
                            break;
                }

                else
                {
                    for (int i = cordShipY - 1; 0 < i; i--)
                        if (HitShoot(cordShipX, i))
                            up_down--;
                        else
                            break;
                }

                if (Form1.DestroyedShip(cordShipX, cordShipY, cordShipX, cordShipY, enemyMap, enemyButtons))
                {
                    AfterShipDestroyed(cordShipX, cordShipY, hit);
                }
            }

            else if (enemyMap[cordX, cordY] == 1)
            {
                enemyMap[cordX, cordY] = 2;
                enemyButtons[cordX, cordY].Text = "X";

                hit = true;

                cordShipX = cordX;
                cordShipY = cordY;

                left_right = 0;
                up_down = 0;

                wasHit = true;

                if (Form1.DestroyedShip(cordX, cordY, cordX, cordY, enemyMap, enemyButtons))
                {
                    AfterShipDestroyed(cordX, cordY, hit);
                }
            }

            else 
                enemyButtons[cordX, cordY].BackColor = Color.Gray;

            if (hit)
            {
                Thread.Sleep(1);
                BotShoot();
            }

            return hit;
        }
    }
}
