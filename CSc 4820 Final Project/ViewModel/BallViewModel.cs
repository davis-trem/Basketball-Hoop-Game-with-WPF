using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CSc_4820_Final_Project.ViewModel
{
    class BallViewModel
    {

        public void MoveBall(double posX, double posY, Image ball)
        {
            var top = Canvas.GetTop(ball);
            var left = Canvas.GetLeft(ball);

            TranslateTransform trans = new TranslateTransform();
            ball.RenderTransform = trans;
            
            double duration = 0.6;

            DoubleAnimation throwYAnimation = new DoubleAnimation(top, posY+top, new Duration(TimeSpan.FromSeconds(duration/1.5)));
            throwYAnimation.AutoReverse = true;

            DoubleAnimation throwXAnimation = new DoubleAnimation(left, posX+left, new Duration(TimeSpan.FromSeconds(duration)));
            throwXAnimation.FillBehavior = FillBehavior.Stop;
            
            throwXAnimation.Completed += (s, e) => {
                Point ballPoint = ball.TransformToAncestor(Application.Current.MainWindow)
                          .Transform(new Point(0, 0));
                //Console.WriteLine("Done at points: "+ ballPoint);

                //Console.WriteLine("Done at top: " + Canvas.GetTop(ball));
                //Console.WriteLine("Done at left: " + Canvas.GetLeft(ball));
            };
            

            ball.BeginAnimation(Canvas.TopProperty, throwYAnimation);
            ball.BeginAnimation(Canvas.LeftProperty, throwXAnimation);
            
        }

        public void MoveHoop(Ellipse hoop)
        {
            var top = Canvas.GetTop(hoop);
            var left = Canvas.GetLeft(hoop);
            TranslateTransform trans = new TranslateTransform();
            hoop.RenderTransform = trans;

            double duration = 2;

            DoubleAnimation hoopYAnimation = new DoubleAnimation(top, 390, new Duration(TimeSpan.FromSeconds(duration)));
            hoopYAnimation.AutoReverse = true;
            hoopYAnimation.RepeatBehavior = RepeatBehavior.Forever;


            hoop.BeginAnimation(Canvas.TopProperty, hoopYAnimation);
        }
        public void StopHoop(Ellipse hoop)
        {
            hoop.BeginAnimation(Canvas.TopProperty, null);
        }
        

    }
}
