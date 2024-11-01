using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using OpenTK.WinForms;
using OpenTK.Windowing.Common;

namespace AnimatedText
{
    public class MainForm : Form
    {
        // объект для работы с трехмерной графикой в приложении
        private GraphicsManager graphicsManager;

        // элементы управления формы
        private Label bitmapTextLabel;
        private TextBox textInputField;
        private Button buttonOK;
        private GLControl glControl;
        private TrackBar speedTrackBar;
        private Label speedLabel;
        private CheckBox CheckBoxX;
        private CheckBox checkBoxY;
        private CheckBox checkBoxZ;
        private Label axisLabel;
        private Label leftEndpointLabel;
        private Label rightEndpointLabel;
        // экземпляр для фонового потока выполнения
        private BackgroundWorker mainLoopWorker;

        public MainForm()
        {
            // Метод, автоматически сгенерированный дизайнером WinForms,
            // для инициализации компонентов формы
            InitializeComponent();

            graphicsManager = new GraphicsManager();
            mainLoopWorker = new BackgroundWorker();

            // добавление делегата для выполнения фоновой задачи в отдельном потоке
            mainLoopWorker.DoWork += MainLoopWorker_DoWork;
            mainLoopWorker.WorkerSupportsCancellation = true;
        }

        // отрисовка кадра
        private void update()
        {
            // проверка, отличается ли текущий поток от главного потока (UI thread)
            if (InvokeRequired)
            {
                try
                {
                    // вызвов метода в главном потоке
                    Invoke(new Action(update));
                }
                catch (ObjectDisposedException)
                {
                    // предотвращение дальнейших операций с удаленным объектом
                    return;
                }

                // Предотвращение дальнейшего выполнения метода в потоке,
                // отличном от главного
                return;
            }

            // использование контекста OpenGL как текущего контекста для рендеринга
            glControl.MakeCurrent();

            // анимация и отрисовка текстуры
            graphicsManager.AnimateTexture();
            graphicsManager.DrawTexturedQuad();

            // отображение кадра с использованием двойной буферизации
            glControl.SwapBuffers();
        }

        // завершает фоновую задачу при закрытии формы
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (mainLoopWorker != null && mainLoopWorker.IsBusy)
            {
                // завершение фонового потока
                mainLoopWorker.CancelAsync();

                // ожидание завершения программы
                while (mainLoopWorker.IsBusy)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        // освобождает графические ресурсы по завершении программы
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                graphicsManager?.Dispose();

            base.Dispose(disposing);
        }

        // код приложения, выполняемый в фоновом потоке
        private void MainLoopWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!mainLoopWorker.CancellationPending)
            {
                // отрисовка кадра
                update();

                // Приостанавливает выполнение текущего потока на 16 миллисекунд
                // для отрисовки с частотой 60 кадров в секунду.
                System.Threading.Thread.Sleep(16);
            }
        }

        // ====== обработчики событий ======
        // метод, выполняемый при инициализации приложения
        private void MainForm_Load(object sender, EventArgs e)
        {
            // настройка начальных параметров
            graphicsManager.Initialize(glControl.Width, glControl.Height);
            graphicsManager.GenerateTexture(textInputField.Text);

            // запуск основного цикла отрисовки
            mainLoopWorker.RunWorkerAsync();
        }

        // обработчик при нажатии на кнопку для обновления текста
        private void buttonOK_Click(object sender, EventArgs e)
        {
            // обновляет текстуру на основе текста из поля для ввода
            string newText = textInputField.Text;
            graphicsManager.GenerateTexture(newText);
        }

        // обработчик для флажка с указанием оси вращения
        private void checkBoxX_CheckedChanged(object sender, EventArgs e)
        {
            // задает вращение по оси X
            graphicsManager.ToggleRotationAxisX(CheckBoxX.Checked);
        }

        // обработчик для флажка с указанием оси вращения
        private void checkBoxY_CheckedChanged(object sender, EventArgs e)
        {
            // задает вращение по оси Y
            graphicsManager.ToggleRotationAxisY(checkBoxY.Checked);
        }

        // обработчик для флажка с указанием оси вращения
        private void checkBoxZ_CheckedChanged(object sender, EventArgs e)
        {
            // задает вращение по оси Z
            graphicsManager.ToggleRotationAxisZ(checkBoxZ.Checked);
        }

        // обработчик для ползунка с указанием скорости движения
        private void speedTrackBar_Scroll(object sender, EventArgs e)
        {
            // задание скорости вращения текстуры
            graphicsManager.SetRotationSpeed((float)speedTrackBar.Value);
        }

        // Метод, автоматически сгенерированный дизайнером WinForms,
        // для инициализации компонентов формы
        private void InitializeComponent()
        {
            this.bitmapTextLabel = new System.Windows.Forms.Label();
            this.textInputField = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();

            // подключение фиксированного конвейера OpenGL для OpenTK
            GLControlSettings settings =
                new GLControlSettings() { Profile = ContextProfile.Compatability };
            this.glControl = new GLControl(settings);

            this.speedTrackBar = new System.Windows.Forms.TrackBar();
            this.speedLabel = new System.Windows.Forms.Label();
            this.CheckBoxX = new System.Windows.Forms.CheckBox();
            this.checkBoxY = new System.Windows.Forms.CheckBox();
            this.checkBoxZ = new System.Windows.Forms.CheckBox();
            this.axisLabel = new System.Windows.Forms.Label();
            this.leftEndpointLabel = new System.Windows.Forms.Label();
            this.rightEndpointLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar))
                .BeginInit();
            this.SuspendLayout();
            //
            // bitmapTextLabel
            //
            this.bitmapTextLabel.AutoSize = true;
            this.bitmapTextLabel.Location = new System.Drawing.Point(1030, 19);
            this.bitmapTextLabel.Margin =
                new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.bitmapTextLabel.Size = new System.Drawing.Size(108, 20);
            this.bitmapTextLabel.TabIndex = 5;
            this.bitmapTextLabel.Text = "Укажите текст:";
            //
            // textInputField
            //
            this.textInputField.Location = new System.Drawing.Point(1030, 43);
            this.textInputField.Margin = new System.Windows.Forms.Padding(4);
            this.textInputField.Size = new System.Drawing.Size(219, 26);
            this.textInputField.TabIndex = 4;
            this.textInputField.Text = "Hello World!";
            //
            // buttonOK
            //
            this.buttonOK.Location = new System.Drawing.Point(1030, 77);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOK.Size = new System.Drawing.Size(219, 29);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            //
            // glControl
            //
            this.glControl.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
            this.glControl.APIVersion = new System.Version(3, 2, 0, 0);
            this.glControl.BackColor = System.Drawing.Color.Black;
            this.glControl.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            this.glControl.IsEventDriven = true;
            this.glControl.Location = new System.Drawing.Point(10, 10);
            this.glControl.Size = new System.Drawing.Size(1000, 655);
            this.glControl.TabIndex = 19;
            //
            // speedTrackBar
            //
            this.speedTrackBar.Location = new System.Drawing.Point(1030, 188);
            this.speedTrackBar.Minimum = 1;
            this.speedTrackBar.Size = new System.Drawing.Size(227, 53);
            this.speedTrackBar.TabIndex = 7;
            this.speedTrackBar.Value = 4;
            this.speedTrackBar.Scroll +=
                new System.EventHandler(this.speedTrackBar_Scroll);
            //
            // speedLabel
            //
            this.speedLabel.AutoSize = true;
            this.speedLabel.Location = new System.Drawing.Point(1030, 165);
            this.speedLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.speedLabel.Size = new System.Drawing.Size(76, 20);
            this.speedLabel.TabIndex = 8;
            this.speedLabel.Text = "Скорость:";
            //
            // CheckBoxX
            //
            this.CheckBoxX.AutoSize = true;
            this.CheckBoxX.Checked = true;
            this.CheckBoxX.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxX.Location = new System.Drawing.Point(1040, 276);
            this.CheckBoxX.Size = new System.Drawing.Size(37, 24);
            this.CheckBoxX.TabIndex = 10;
            this.CheckBoxX.Text = "X";
            this.CheckBoxX.UseVisualStyleBackColor = true;
            this.CheckBoxX.CheckedChanged +=
                new System.EventHandler(this.checkBoxX_CheckedChanged);
            //
            // checkBoxY
            //
            this.checkBoxY.AutoSize = true;
            this.checkBoxY.Checked = true;
            this.checkBoxY.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxY.Location = new System.Drawing.Point(1125, 276);
            this.checkBoxY.Size = new System.Drawing.Size(36, 24);
            this.checkBoxY.TabIndex = 11;
            this.checkBoxY.Text = "Y";
            this.checkBoxY.UseVisualStyleBackColor = true;
            this.checkBoxY.CheckedChanged +=
                new System.EventHandler(this.checkBoxY_CheckedChanged);
            //
            // checkBoxZ
            //
            this.checkBoxZ.AutoSize = true;
            this.checkBoxZ.Checked = true;
            this.checkBoxZ.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxZ.Location = new System.Drawing.Point(1210, 276);
            this.checkBoxZ.Size = new System.Drawing.Size(37, 24);
            this.checkBoxZ.TabIndex = 12;
            this.checkBoxZ.Text = "Z";
            this.checkBoxZ.UseVisualStyleBackColor = true;
            this.checkBoxZ.CheckedChanged +=
                new System.EventHandler(this.checkBoxZ_CheckedChanged);
            //
            // axisLabel
            //
            this.axisLabel.AutoSize = true;
            this.axisLabel.Location = new System.Drawing.Point(1030, 253);
            this.axisLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.axisLabel.Size = new System.Drawing.Size(113, 20);
            this.axisLabel.TabIndex = 13;
            this.axisLabel.Text = "Ось вращения:";
            //
            // leftEndpointLabel
            //
            this.leftEndpointLabel.AutoSize = true;
            this.leftEndpointLabel.Location = new System.Drawing.Point(1036, 221);
            this.leftEndpointLabel.Margin
                = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.leftEndpointLabel.Size = new System.Drawing.Size(17, 20);
            this.leftEndpointLabel.TabIndex = 14;
            this.leftEndpointLabel.Text = "1";
            //
            // rightEndpointLabel
            //
            this.rightEndpointLabel.AutoSize = true;
            this.rightEndpointLabel.Location = new System.Drawing.Point(1232, 221);
            this.rightEndpointLabel.Margin
                = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.rightEndpointLabel.Size = new System.Drawing.Size(25, 20);
            this.rightEndpointLabel.TabIndex = 15;
            this.rightEndpointLabel.Text = "10";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1262, 675);
            this.Controls.Add(this.rightEndpointLabel);
            this.Controls.Add(this.leftEndpointLabel);
            this.Controls.Add(this.axisLabel);
            this.Controls.Add(this.checkBoxZ);
            this.Controls.Add(this.checkBoxY);
            this.Controls.Add(this.CheckBoxX);
            this.Controls.Add(this.speedLabel);
            this.Controls.Add(this.speedTrackBar);
            this.Controls.Add(this.glControl);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textInputField);
            this.Controls.Add(this.bitmapTextLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Графическое моделирование движения текста";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.speedTrackBar))
                .EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
