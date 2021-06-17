using ClassTransformer.Translator;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ClassTransformer
{
    public partial class Form1 : Form
    {
        public IEnumerable<ILanguageSource> Sources = new List<ILanguageSource>()
        {
            new CSSource(),
        };
        public IEnumerable<ILanguageTarget> Targets = new List<ILanguageTarget>()
        {
            new TSTarget(),
        };

        public ILanguageSource SelectedSource { get; set; }
        public ILanguageTarget SelectedTarget { get; set; }

        private StringifyConfig stringifyConfig = new StringifyConfig()
        {
            ConvertClassToInteface = true,
        };

        public Form1()
        {
            InitializeComponent();
            comboBox1.DataSource = Sources;
            comboBox2.DataSource = Targets;
            comboBox1.DisplayMember = "Label";
            comboBox2.DisplayMember = "Label";
            classToInterfaceCheckBox.Checked = stringifyConfig.ConvertClassToInteface;
        }

        private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SelectedSource = comboBox1.SelectedItem as ILanguageSource;
            Translate();
        }

        private void comboBox2_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            SelectedTarget = comboBox2.SelectedItem as ILanguageTarget;
            Translate();
        }

        private void richTextBox1_TextChanged(object sender, System.EventArgs e)
        {
            Translate();
        }

        private void Translate()
        {
            if (SelectedSource == null || SelectedTarget == null)
            {
                return;
            }

            var text = richTextBox1.Text;
            var classes = SelectedSource.GetClasses(text);
            richTextBox2.Text = SelectedTarget.Stringify(classes, stringifyConfig);
        }

        private void classToInterfaceCheckBox_CheckedChanged(object sender, System.EventArgs e)
        {

        }
    }
}
