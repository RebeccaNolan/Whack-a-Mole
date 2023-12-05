using Plugin.Maui.Audio;

namespace Nolan_Rebecca_VS22;

public partial class MainPage : ContentPage
{
    private System.Timers.Timer timer;
    private System.Timers.Timer moleTimer;
    private IAudioManager audioManager;

    //Variables
    private int points = 0;
    private int highScore;
    private int interval = 1000;
    private int interval2 = 1500;
    private int moleCount = 1;
    private int countdown = 20;
    private bool gridFour = true;
    private bool gameRunning = false;
    private int maxRows = 4;

    private Random random;

    public MainPage(IAudioManager audioManager)
	{
		InitializeComponent();
        this.audioManager = audioManager;
        random = new Random();
        setTimer();
        SetMoleTimer();
        InitializeHighScore();
    }

    private void setTimer()
    {
        //Create a timer with an optional Interval
        timer = new System.Timers.Timer

        {
            Interval = interval
        };
        timer.Elapsed += Timer_Elapsed;
    }

    private void Timer_Elapsed(object sender, EventArgs e)
    {
        Dispatcher.Dispatch(
            () =>
            {
                TimerFunction();
            }
            );
    }

    private void TimerFunction()
    {
        if (countdown > 0)
        {
            --countdown;
            timer_label.Text = countdown.ToString();
        }
        else
        {
            EndGame();
        }
    }

    private void SetMoleTimer()
    {
        //Sets up a timer so the mole diappears if user doesn't tap it in time
        moleTimer = new System.Timers.Timer
        {
            Interval = interval2,
        };
        moleTimer.Elapsed += MoleTimer_Elapsed;
    }


    private void MoleTimer_Elapsed(object sender, EventArgs e)
    {
        Dispatcher.Dispatch(
           () =>
           {
               TimerMoleFunction();
           }
           );
    }

    private void TimerMoleFunction()
    {
        if (moleCount > 0)
        {
            --moleCount;
        }
        else if (moleCount == 0)
        {
            if (gridFour)
            {
                MoveTheMole(mole_image4);
            }
            else
            {
                MoveTheMole(mole_image5);
                moleCount = 1;
            }
        }
    }

    private void StartGame()
    {
        //Starts the Game, moves the mole, set everything to 0
        moleTimer.Stop();
        timer.Stop();
        points = 0;
        points_label.Text = points.ToString();

        countdown = 20;
        timer_label.Text = countdown.ToString();

        gameRunning = true;

        if (gridFour)
        {
            mole_image4.IsVisible = true;
            MoveTheMole(mole_image4);
        }
        else
        {
            mole_image5.IsVisible = true;
            MoveTheMole(mole_image5);
        }

        timer.Start();
        moleTimer.Start();
    }

    private async void MoveTheMole(Image img)
    {
        //moves the mole randomly within the grid
        int r, c;
        r = random.Next(0, maxRows);
        c = random.Next(0, maxRows);
        img.SetValue(Grid.RowProperty, r);
        img.SetValue(Grid.ColumnProperty, c);
        //Animation for mole
        img.Opacity = 100;
        img.Scale = 0;
        img.FadeTo(1, 500);
        await img.ScaleTo(1, 500);
    }

    private async void ImageTapped(object sender, EventArgs e)
    {
        if (!gameRunning)
        {
            return;
        }//Blocks any tapping of images
        var player = audioManager.CreatePlayer(await FileSystem.OpenAppPackageFileAsync("sound-effects-library-jingle-got-item.mp3"));
        player.Play();//plays sound effect when image is tapped
        IncreaseScore();
        MoveTheMole((Image)sender);
        moleTimer.Stop();
        moleTimer.Start();
    }

    private void StartBtn_Clicked(object sender, EventArgs e)
    {
        //Starts the game
        if (!gameRunning)
        {
            StartGame();
        }
    }

    private async void ResetGame()
    {
        //If user wants to reset the game - asks user to confirm
        if (gameRunning)
        {
            bool confirm = await DisplayAlert("Confirm", "Are you sure?", "Yes", "No");
            if (!confirm)
            {
                return;
            }
        }

        //resets everything if they say yes
        timer.Stop();
        points = 0;
        points_label.Text = points.ToString();
        countdown = 20;
        timer_label.Text = countdown.ToString();
        gameRunning = false;
        mole_image4.IsVisible = false;
        mole_image5.IsVisible = false;
    }

    private void ResetGame_Clicked(object sender, EventArgs e)
    {
        ResetGame();
    }

    private void InitializeHighScore()
    {
        highScore = Preferences.Default.Get("High_Score", 0);
        HighScore_Label.Text = "High Score: " + highScore;
    }
    private void IncreaseScore()
    {
        ++points;
        points_label.Text = points.ToString();
    }

    private void SwitchGridBtn_Clicked(object sender, EventArgs e)
    {
        if (gridFour && !gameRunning)
        {
            //Enable 4x4 Grid
            Grid4.IsVisible = false;
            Grid4.IsEnabled = false;

            //Hide 5x5
            Grid5.IsVisible = true;
            Grid5.IsEnabled = true;

            gridFour = false;
            maxRows = 5;

        }
        else if (!gridFour && !gameRunning)
        {
            //Enable 5x5 Grid
            Grid4.IsVisible = true;
            Grid4.IsEnabled = true;

            //Hide 4x4
            Grid5.IsVisible = false;
            Grid5.IsEnabled = false;

            gridFour = true;
            maxRows = 4;
        }
    }

    private void EndGame()
    {
        //Stops and hides everything
        timer.Stop();
        moleTimer.Stop();
        gameRunning = false;
        mole_image4.IsVisible = false;
        mole_image5.IsVisible = false;

        if (points > highScore)
        {
            highScore = points;
            HighScore_Label.Text = "High Score: " + highScore;
            Preferences.Default.Set("High_Score", highScore);//write high score
        }//to update highscore
    }
}

/* Audio plugin learned from YouTube Video -
 * Play Sound in .NET MAUI with Plugin.Maui.Audio -- Gerald Versluis
 * Sound Effect: https://www.free-stock-music.com/
 * Mole Image created with Adobe Illustrator
 */