using Floppy.Properties;
using System;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;

namespace Floppy
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        MediaPlayer player = new MediaPlayer();

        int lifeCount;

        int gravity = 8;

        DispatcherTimer gameTimer = new DispatcherTimer();

        double score;

        double volume;

        double jumpCount;

        Rect pillars;

        Rect FlappyRect;

        bool gameover = false;

        public MainWindow(bool music)
        {
            InitializeComponent();
            gameTimer.Tick += gameEngine; 
            gameTimer.Interval = TimeSpan.FromMilliseconds(20); 
            gameStart();
            Music_checked(music);
        }

        private void gameStart()
        {

            lifeCount = 1;

            int temp = 200;
          
            score = 0;

            jumpCount = 0;
            
            gameover = false;

            Canvas.SetTop(flappyBird, 100);

                //Задаём изначальное положение объектов на экране
                foreach (var x in MyCanvas.Children.OfType<Image>())
                {

                if (x is Image && (string)x.Tag == "obs1")
                {
                    Canvas.SetLeft(x, 500);
                }
                
                if (x is Image && (string)x.Tag == "obs2")
                {
                    Canvas.SetLeft(x, 800);
                }
                
                if (x is Image && (string)x.Tag == "obs3")
                {
                    Canvas.SetLeft(x, 1000);
                }
                
                if (x is Image && (string)x.Tag == "clouds")
                {
                    Canvas.SetLeft(x, (300 + temp));
                    temp = 800;
                }

                if (x is Image && (string)x.Tag == "obj1")
                {
                    Canvas.SetLeft(x, 500);
                }
                if (x is Image && (string)x.Tag == "obj2")
                {
                    Canvas.SetLeft(x, 800);
                }
                if (x is Image && (string)x.Tag == "obj3")
                {
                    Canvas.SetLeft(x, 1000);
                }
                if (x is Image && (string)x.Tag == "backgroundCity")
                {
                    Canvas.SetLeft(x, 0);
                }
                if (x is Image && (string)x.Tag == "backgroundOut")
                {
                    Canvas.SetLeft(x, 1560);
                }
            }
            gameTimer.Start();
        }

        private void gameEngine(object sender, EventArgs e)
        {
            flappyBird.Source = new BitmapImage(new Uri(@"/BirdUp.png", UriKind.Relative));

            distText.Content = ": " + jumpCount;
            scoreText.Content = ": " + score;
            healthText.Content = ": " + lifeCount;
            
            FlappyRect = new Rect(Canvas.GetLeft(flappyBird), Canvas.GetTop(flappyBird), flappyBird.Width - 12, flappyBird.Height - 6);

            Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + gravity);

            Health_Up();

            if (Canvas.GetTop(flappyBird) + flappyBird.Height > 490 || Canvas.GetTop(flappyBird) < -30)
            {
                    Canvas.SetTop(flappyBird, Canvas.GetTop(flappyBird) + 300);
                    lifeCount = lifeCount - 1;

                if (lifeCount == 0)
                {
                    //Ушли за пределы экрана? Проиграли - Выводим доступные клавишы.
                    healthText.Content = ": 0";
                    gameover = true;
                    gameTimer.Stop();
                    goText.Visibility = Visibility.Visible;
                    player.Open(new Uri($"{Environment.CurrentDirectory}\\muzak\\crashGO.wav"));
                    player.Volume = 0.1;
                    player.Play();
                }
            }
            
            foreach (var x in MyCanvas.Children.OfType<Image>())
            {

                if ((string)x.Tag == "obs1" || (string)x.Tag == "obs2" || (string)x.Tag == "obs3")
                {

                    Rect pillars = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);

                    if (FlappyRect.IntersectsWith(pillars))
                    {
                            Canvas.SetLeft(x, 800);
                            lifeCount = lifeCount - 1;

                        if (lifeCount == 0)
                        {
                            healthText.Content = ": 0";
                            gameover = true;
                            gameTimer.Stop();
                            //Ударились об преграду - проиграли. Выводим доступные клавишы.
                            flappyBird.Source = new BitmapImage(new Uri(@"/BirdHit.png", UriKind.Relative));
                            goText.Visibility = Visibility.Visible;
                            player.Open(new Uri($"{Environment.CurrentDirectory}\\muzak\\gameOver.wav"));
                            player.Volume = 0.1;
                            player.Play();
                        } 
                    }
                }
                
                //Если преграда ушла за экран - телепортируем её

                if ((string)x.Tag == "obs1" && Canvas.GetLeft(x) < -100)
                {
                    
                    Canvas.SetLeft(x, 800);
                   
                }
                
                if ((string)x.Tag == "obs2" && Canvas.GetLeft(x) < -200)
                {
                   
                    Canvas.SetLeft(x, 800);
                    
                }
                
                if ((string)x.Tag == "obs3" && Canvas.GetLeft(x) < -250)
                {
                    
                    Canvas.SetLeft(x, 800);
                    
                }

                if ((string)x.Tag == "obj1" || (string)x.Tag == "obj2" || (string)x.Tag == "obj3")
                {

                    Rect gold = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 5);

                    if (FlappyRect.IntersectsWith(gold))
                    {
                        // Собрали золото - получили очки
                        score += 1;
                        gravity += -1;
                        Canvas.SetLeft(x, 800);
                    } 
                    
                    if (Canvas.GetLeft(x) < -100)
                    {
                        Canvas.SetLeft(x, 800);
                    }

                    if (gold.IntersectsWith(pillars))
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + 100);
                    }

                }

                //Сдвиг облаков по экрану
                if ((string)x.Tag == "clouds")
                {
                    
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - .6);
                    
                    if (Canvas.GetLeft(x) < -220)
                    {
                        
                        Canvas.SetLeft(x, 550);
                    }
                }

                //Сдвиг заднего фона
                if ((string)x.Tag == "backgroundCity")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - .6);
                    
                    if (Canvas.GetLeft(x) < -1569)
                    {
                        Canvas.SetLeft(x, 1440);
                    }
                }

                if ((string)x.Tag == "backgroundOut")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - .6);

                    if (Canvas.GetLeft(x) < -1440)
                    {
                        Canvas.SetLeft(x, 1569);
                    }
                }
            }
        }

        //Функционал прыжка и кнопок, отвечающих за перезапуск и выход из игры


        private void Jump(object sender, KeyEventArgs e)
        {
            jumpCount++;
            flappyBird.RenderTransform = new RotateTransform(-20, flappyBird.Width / 3, flappyBird.Height / 3);
            flappyBird.Source = new BitmapImage(new Uri(@"/BirdDown.png", UriKind.Relative));

            gravity = -12;

        }

        private void Health_Up()
        {
            if (score >= 5 && lifeCount < 5)
            {
                score -= 5;
                lifeCount++;
                gravity += 5;
            }
        }

        private void Background_Music(double volume)
        {
            player.Open(new Uri($"{Environment.CurrentDirectory}\\muzak\\bgMuzak2.wav"));
            player.Volume = volume;
            player.Play();
        }

        private void Music_checked(bool music)
        {

                if (music)
                {
                volume = 0.1;
                Background_Music(volume);
                }  
                else
                {
                volume = 0;
                Background_Music(volume);
            }
        }

        private void Canvas_KeyIsDown(object sender, KeyEventArgs e)
        {

            if (!gameover)
            {
                if (e.Key == Key.Space && !e.IsRepeat)
                {
                    Jump(sender, e);
                }
                else if (e.Key == Key.Space && e.IsRepeat)
                {
                    Canvas_KeyIsUp(sender, e);
                } 
            }
            
            if (gameover)
            {

                if (e.Key == Key.R)
                {
                    gameStart();
                    Background_Music(volume);
                    goText.Visibility = Visibility.Hidden;
                }

                if (e.Key == Key.X)
                {
                    player.Stop();

                    Close();
                }
            }

        }

        private void Canvas_KeyIsUp(object sender, KeyEventArgs e)
        {

            if (!gameover)
            {
                    flappyBird.RenderTransform = new RotateTransform(10, flappyBird.Width / 3, flappyBird.Height / 3);
                    flappyBird.Source = new BitmapImage(new Uri(@"/BirdUp.png", UriKind.Relative));

                    gravity = 6;
            }
            
        }

    }
}
