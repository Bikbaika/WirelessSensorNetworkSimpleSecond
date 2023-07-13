using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WirelessSensorNetworkSimpleSecond
{
        public partial class AddNodeDialog : Form
        {
            public string SelectedNodeType { get; private set; }
            public Point SelectedNodeLocation { get; set; }

            public AddNodeDialog()
            {
             //   InitializeComponent();
                InitializeControls();
            }

            private void InitializeControls()
            {
                // Создаем элементы управления для выбора типа узла и указания позиции

                var lblNodeType = new Label
                {
                    Location = new Point(10, 10),
                    Text = "Выберите тип узла:"
                };

                var cmbNodeType = new ComboBox
                {
                    Location = new Point(10, 30),
                    Items = { "Sensor", "BaseStation" },
                    Width = 100
                };

                var lblNodeLocation = new Label
                {
                    Location = new Point(10, 60),
                    Text = "Укажите позицию узла:"
                };

                var numNodeX = new NumericUpDown
                {
                    Location = new Point(10, 80),
                    Width = 50,
                    Minimum = 0,
                    Maximum = 1000
                };

                var numNodeY = new NumericUpDown
                {
                    Location = new Point(70, 80),
                    Width = 50,
                    Minimum = 0,
                    Maximum = 1000
                };

                var btnOK = new Button
                {
                    Text = "OK",
                    Location = new Point(10, 120),
                    DialogResult = DialogResult.OK
                };

                var btnCancel = new Button
                {
                    Text = "Cancel",
                    Location = new Point(90, 120),
                    DialogResult = DialogResult.Cancel
                };

                // Добавляем элементы управления на форму
                Controls.AddRange(new Control[] { lblNodeType, cmbNodeType, lblNodeLocation, numNodeX, numNodeY, btnOK, btnCancel });

                // Обработчик события нажатия кнопки OK
                btnOK.Click += (sender, e) =>
                {
                    SelectedNodeType = cmbNodeType.SelectedItem?.ToString();
                    SelectedNodeLocation = new Point((int)numNodeX.Value, (int)numNodeY.Value);
                };
            }
        }
    }

