using System.Windows;
using System.Windows.Controls;

namespace Akinator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private AkinatorGame _akinator;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Добавляет сообщение в игровой чат.
        /// </summary>
        /// <param name="sender">Отправитель сообщения</param>
        /// <param name="message">Текст сообщения</param>
        private void AddMessageToChat(string sender, string message)
        {
            string formattedMessage = $"{DateTime.Now.ToShortTimeString()} | {sender}: {message}";

            GameChat.AppendText(formattedMessage + Environment.NewLine);
            GameChat.ScrollToEnd();
        }

        /// <summary>
        /// Вызывается при смене вопроса.
        /// </summary>
        /// <param name="question">Текст вопроса</param>
        private void UpdateGameChat(string question)
        {
            AddMessageToChat("Игра", question);
        }

        /// <summary>
        /// Вызывается при окончании игры.
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        private void EndGame(string message)
        {
            AddMessageToChat("Игра", message);
            if (message.Contains("Сдаюсь"))
            {
                YesButton.IsEnabled = false;
                NoButton.IsEnabled = false;

                NewGiftTextBox.Visibility = Visibility.Visible;
                NewQuestionTextBox.Visibility = Visibility.Visible;
                SubmitNewDataButton.Visibility = Visibility.Visible;
                YesNoChoicePanel.Visibility = Visibility.Visible;
            }
            else
            {
                YesButton.IsEnabled = false;
                NoButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Вызывается при отображении базы знаний.
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        private void PrintKnowledgeBase(string message)
        {
            GameChat.AppendText(message);
            GameChat.ScrollToEnd();
        }

        private void NewGameButton_OnClick(object sender, RoutedEventArgs e)
        {
            _akinator = new AkinatorGame();
            _akinator.OnQuestionUpdated += UpdateGameChat;
            _akinator.OnGameOver += EndGame;
            _akinator.OnKnowledgeBaseShow += PrintKnowledgeBase;

            GameChat.Text = string.Empty;
            YesButton.IsEnabled = true;
            NoButton.IsEnabled = true;
            ShowKnowledgeBaseButton.IsEnabled = true;
            _akinator.StartNewGame();
        }

        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddMessageToChat("Игрок", "Да");
            _akinator.HandleAnswer(true);
        }

        private void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            AddMessageToChat("Игрок", "Нет");
            _akinator.HandleAnswer(false);
        }

        private void SubmitNewDataButton_OnClick(object sender, RoutedEventArgs e)
        {
            string newGift = NewGiftTextBox.Text;
            string newQuestion = NewQuestionTextBox.Text;

            if (!string.IsNullOrEmpty(newGift) && !string.IsNullOrEmpty(newQuestion))
            {
                bool isYesAnswer = YesOption.IsChecked == true;
                _akinator.LearnNewGift(newGift, newQuestion, isYesAnswer);

                NewGiftTextBox.Visibility = Visibility.Collapsed;
                NewQuestionTextBox.Visibility = Visibility.Collapsed;
                SubmitNewDataButton.Visibility = Visibility.Collapsed;
                YesNoChoicePanel.Visibility = Visibility.Collapsed;

                AddMessageToChat("Игра", "Новый подарок добавлен в базу знаний!");
            }
            else
            {
                AddMessageToChat("Игра", "Пожалуйста, заполните оба поля!");
            }
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            GetWindow(this)?.Close();
        }

        private void ShowKnowledgeBaseButton_Click(object sender, RoutedEventArgs e)
        {
            //AddMessageToChat("Игра", "Дерево базы знаний:");
            //_akinator.ShowKnowledgeBase();
            ShowKnowledgeBase(_akinator.RootNode);
        }

        private void ShowKnowledgeBase(Node rootNode)
        {
            KnowledgeBase.Items.Clear();
            var rootItem = new TreeViewItem {Header = rootNode.Data};
            
            AddTreeItems(rootItem, rootNode);
            
            KnowledgeBase.Items.Add(rootItem);
        }

        private void AddTreeItems(TreeViewItem parentItem, Node currentNode)
        {
            if (currentNode.IsQuestion)
            {
                // Если узел - это вопрос, добавляем ветви "да" и "нет"
                var yesItem = new TreeViewItem { Header = "Да: " + currentNode.YesBranch?.Data };
                var noItem = new TreeViewItem { Header = "Нет: " + currentNode.NoBranch?.Data };

                parentItem.Items.Add(yesItem);
                parentItem.Items.Add(noItem);

                // Рекурсивно добавляем дочерние элементы
                if (currentNode.YesBranch != null) AddTreeItems(yesItem, currentNode.YesBranch);
                if (currentNode.NoBranch != null) AddTreeItems(noItem, currentNode.NoBranch);
            }
        }
    }
}