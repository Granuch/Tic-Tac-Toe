using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Tic_Tac_Toe;

namespace Tic_Tac_Toe.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private readonly GameEngine _engine;

        public ObservableCollection<string> Board { get; set; }

        public ICommand CellClickCommand { get; }
        public ICommand RestartCommand { get; }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public GameViewModel()
        {
            _engine = new GameEngine();
            Board = new ObservableCollection<string>(new string[9]);

            CellClickCommand = new RelayCommand(OnCellClicked);
            RestartCommand = new RelayCommand(_ => Restart());

            StatusText = "Player X's turn";
        }

        private void OnCellClicked(object param)
        {
            int index = int.Parse(param.ToString());

            if (!_engine.MakeMove(index))
                return;

            UpdateBoard();

            if (_engine.CheckWinner())
            {
                StatusText = $"Player {_engine.CurrentPlayer} wins!";
                return;
            }

            if (_engine.CheckDraw())
            {
                StatusText = "Draw!";
                return;
            }

            _engine.SwitchPlayer();
            StatusText = $"Player {_engine.CurrentPlayer}'s turn";
        }

        private void Restart()
        {
            _engine.ResetBoard();
            UpdateBoard();
            StatusText = "Player X's turn";
        }

        private void UpdateBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                Board[i] = _engine.Board[i] == '\0' ? "" : _engine.Board[i].ToString();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _action;

        public RelayCommand(Action<object> action) => _action = action;

        public bool CanExecute(object parameter) => true;

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter) => _action(parameter);
    }
}