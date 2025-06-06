using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace OscillatorsSimulation
{
    public class Form1 : Form
    {
        // UI Controls
        private ComboBox oscillatorTypeComboBox;
        private Label amplitudeLabel;
        private NumericUpDown amplitudeNumeric;
        private Label frequencyLabel;
        private NumericUpDown frequencyNumeric;
        private Label dampingLabel;
        private NumericUpDown dampingNumeric;
        private Label timeLabel;
        private NumericUpDown timeNumeric;
        private Button startButton;
        private Button stopButton;
        private PictureBox simulationPictureBox;
        private Timer simulationTimer;

        // Simulation variables
        private Oscillator currentOscillator;
        private float time;
        private Bitmap canvas;
        private Graphics graphics;
        private Pen trajectoryPen;
        private PointF[] trajectory;
        private int trajectoryIndex;
        private const int MaxTrajectoryPoints = 500;

        public Form1()
        {
            InitializeComponents();
            InitializeSimulation();
        }

        private void InitializeComponents()
        {
            // Form setup
            this.Text = "Моделирование колебательных систем";
            this.ClientSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Oscillator type selection
            oscillatorTypeComboBox = new ComboBox
            {
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            oscillatorTypeComboBox.Items.AddRange(new object[] { "Гармонический осциллятор", "Математический маятник" });
            oscillatorTypeComboBox.SelectedIndex = 0;
            oscillatorTypeComboBox.SelectedIndexChanged += OscillatorTypeChanged;
            this.Controls.Add(oscillatorTypeComboBox);

            // Amplitude control
            amplitudeLabel = new Label
            {
                Text = "Амплитуда:",
                Location = new Point(20, 60),
                Size = new Size(100, 20)
            };
            this.Controls.Add(amplitudeLabel);

            amplitudeNumeric = new NumericUpDown
            {
                Location = new Point(120, 60),
                Size = new Size(100, 20),
                Minimum = 0.1m,
                Maximum = 10,
                Increment = 0.1m,
                DecimalPlaces = 1,
                Value = 1.0m
            };
            this.Controls.Add(amplitudeNumeric);

            // Frequency control
            frequencyLabel = new Label
            {
                Text = "Частота:",
                Location = new Point(20, 90),
                Size = new Size(100, 20)
            };
            this.Controls.Add(frequencyLabel);

            frequencyNumeric = new NumericUpDown
            {
                Location = new Point(120, 90),
                Size = new Size(100, 20),
                Minimum = 0.1m,
                Maximum = 5,
                Increment = 0.1m,
                DecimalPlaces = 1,
                Value = 1.0m
            };
            this.Controls.Add(frequencyNumeric);

            // Damping control
            dampingLabel = new Label
            {
                Text = "Затухание:",
                Location = new Point(20, 120),
                Size = new Size(100, 20)
            };
            this.Controls.Add(dampingLabel);

            dampingNumeric = new NumericUpDown
            {
                Location = new Point(120, 120),
                Size = new Size(100, 20),
                Minimum = 0,
                Maximum = 1,
                Increment = 0.01m,
                DecimalPlaces = 2,
                Value = 0.1m
            };
            this.Controls.Add(dampingNumeric);

            // Time control
            timeLabel = new Label
            {
                Text = "Время (с):",
                Location = new Point(20, 150),
                Size = new Size(100, 20)
            };
            this.Controls.Add(timeLabel);

            timeNumeric = new NumericUpDown
            {
                Location = new Point(120, 150),
                Size = new Size(100, 20),
                Minimum = 0,
                Maximum = 100,
                Increment = 0.1m,
                DecimalPlaces = 1,
                Value = 0
            };
            timeNumeric.ReadOnly = true;
            this.Controls.Add(timeNumeric);

            // Buttons
            startButton = new Button
            {
                Text = "Старт",
                Location = new Point(20, 180),
                Size = new Size(100, 30)
            };
            startButton.Click += StartSimulation;
            this.Controls.Add(startButton);

            stopButton = new Button
            {
                Text = "Стоп",
                Location = new Point(130, 180),
                Size = new Size(100, 30),
                Enabled = false
            };
            stopButton.Click += StopSimulation;
            this.Controls.Add(stopButton);

            // PictureBox for drawing
            simulationPictureBox = new PictureBox
            {
                Location = new Point(250, 20),
                Size = new Size(520, 540),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(simulationPictureBox);

            // Timer for simulation
            simulationTimer = new Timer
            {
                Interval = 50
            };
            simulationTimer.Tick += SimulationStep;
        }

        private void InitializeSimulation()
        {
            canvas = new Bitmap(simulationPictureBox.Width, simulationPictureBox.Height);
            graphics = Graphics.FromImage(canvas);
            graphics.Clear(Color.White);
            simulationPictureBox.Image = canvas;

            trajectory = new PointF[MaxTrajectoryPoints];
            trajectoryIndex = 0;
            trajectoryPen = new Pen(Color.Blue, 2);

            CreateOscillator();
        }

        private void CreateOscillator()
        {
            float amplitude = (float)amplitudeNumeric.Value;
            float frequency = (float)frequencyNumeric.Value;
            float damping = (float)dampingNumeric.Value;

            if (oscillatorTypeComboBox.SelectedIndex == 0)
            {
                currentOscillator = new HarmonicOscillator(amplitude, frequency, damping);
            }
            else
            {
                currentOscillator = new Pendulum(amplitude, frequency, damping);
            }

            currentOscillator.PropertyChanged += OscillatorPropertyChanged;
            time = 0;
            timeNumeric.Value = 0;
            trajectoryIndex = 0;
            graphics.Clear(Color.White);
            simulationPictureBox.Invalidate();
        }

        private void OscillatorTypeChanged(object sender, EventArgs e)
        {
            CreateOscillator();
        }

        private void StartSimulation(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;
            oscillatorTypeComboBox.Enabled = false;
            amplitudeNumeric.Enabled = false;
            frequencyNumeric.Enabled = false;
            dampingNumeric.Enabled = false;

            simulationTimer.Start();
        }

        private void StopSimulation(object sender, EventArgs e)
        {
            simulationTimer.Stop();

            startButton.Enabled = true;
            stopButton.Enabled = false;
            oscillatorTypeComboBox.Enabled = true;
            amplitudeNumeric.Enabled = true;
            frequencyNumeric.Enabled = true;
            dampingNumeric.Enabled = true;
        }

        private void SimulationStep(object sender, EventArgs e)
        {
            time += 0.05f;
            timeNumeric.Value = (decimal)time;

            currentOscillator.Update(0.05f);

            // Draw trajectory
            int centerX = simulationPictureBox.Width / 2;
            int centerY = simulationPictureBox.Height / 2;

            float x = centerX + currentOscillator.Position * 50;
            float y = centerY;

            if (oscillatorTypeComboBox.SelectedIndex == 1) // Pendulum
            {
                float length = 150;
                x = centerX + length * (float)Math.Sin(currentOscillator.Position);
                y = centerY + length * (float)Math.Cos(currentOscillator.Position);
            }

            trajectory[trajectoryIndex] = new PointF(x, y);
            trajectoryIndex = (trajectoryIndex + 1) % MaxTrajectoryPoints;

            graphics.Clear(Color.White);

            // Draw axes
            graphics.DrawLine(Pens.Black, centerX - 200, centerY, centerX + 200, centerY);
            graphics.DrawLine(Pens.Black, centerX, centerY - 200, centerX, centerY + 200);

            // Draw trajectory
            for (int i = 1; i < MaxTrajectoryPoints; i++)
            {
                int current = (trajectoryIndex + i) % MaxTrajectoryPoints;
                int previous = (current - 1 + MaxTrajectoryPoints) % MaxTrajectoryPoints;

                if (trajectory[previous].X != 0 && trajectory[previous].Y != 0 &&
                    trajectory[current].X != 0 && trajectory[current].Y != 0)
                {
                    graphics.DrawLine(trajectoryPen, trajectory[previous], trajectory[current]);
                }
            }

            // Draw oscillator
            if (oscillatorTypeComboBox.SelectedIndex == 0) // Harmonic oscillator
            {
                graphics.FillEllipse(Brushes.Red, x - 10, y - 10, 20, 20);
                graphics.DrawLine(Pens.Black, centerX, centerY, x, y);
            }
            else // Pendulum
            {
                graphics.DrawLine(Pens.Black, centerX, centerY, x, y);
                graphics.FillEllipse(Brushes.Red, x - 15, y - 15, 30, 30);
            }

            simulationPictureBox.Invalidate();
        }

        private void OscillatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Можно добавить реакцию на изменения свойств осциллятора
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (graphics != null)
                {
                    graphics.Dispose();
                    graphics = null;
                }
                if (canvas != null)
                {
                    canvas.Dispose();
                    canvas = null;
                }
                if (trajectoryPen != null)
                {
                    trajectoryPen.Dispose();
                    trajectoryPen = null;
                }
                if (simulationTimer != null)
                {
                    simulationTimer.Dispose();
                    simulationTimer = null;
                }
            }
            base.Dispose(disposing);
        }
    }

    // Базовый класс осциллятора
    public abstract class Oscillator : INotifyPropertyChanged
    {
        protected float amplitude;
        protected float frequency;
        protected float damping;
        protected float position;
        protected float velocity;

        public event PropertyChangedEventHandler PropertyChanged;

        public Oscillator(float amplitude, float frequency, float damping)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.damping = damping;
            this.position = amplitude;
            this.velocity = 0;
        }

        public float Position
        {
            get { return position; }
            protected set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public abstract void Update(float deltaTime);

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Гармонический осциллятор
    public class HarmonicOscillator : Oscillator
    {
        public HarmonicOscillator(float amplitude, float frequency, float damping)
            : base(amplitude, frequency, damping) { }

        public override void Update(float deltaTime)
        {
            // Уравнение: x'' + 2βx' + ω²x = 0
            float acceleration = -2 * damping * velocity - frequency * frequency * position;

            velocity += acceleration * deltaTime;
            Position += velocity * deltaTime;
        }
    }

    // Математический маятник
    public class Pendulum : Oscillator
    {
        public Pendulum(float amplitude, float frequency, float damping)
            : base(amplitude, frequency, damping) { }

        public override void Update(float deltaTime)
        {
            // Уравнение: θ'' + 2βθ' + (g/L)sinθ = 0
            // Для упрощения считаем g/L = ω²
            float acceleration = -2 * damping * velocity - frequency * frequency * (float)Math.Sin(position);

            velocity += acceleration * deltaTime;
            Position += velocity * deltaTime;
        }
    }
}