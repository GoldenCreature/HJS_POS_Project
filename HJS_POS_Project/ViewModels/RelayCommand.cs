using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HJS_POS_Project.ViewModels
{
    // ICommand 인터페이스를 구현한 클래스
    // View의 버튼 클릭 같은 이벤트를 ViewModel의 메서드와 연결해주는 역할
    public class RelayCommand : ICommand
    {
        // 실제 실행할 메서드를 담는 변수
        private Action _execute;

        // 버튼 활성화/비활성화 여부를 결정하는 메서드를 담는 변수
        // null이면 항상 활성화
        private Func<bool> _canExecute;

        // 생성자 - 실행할 메서드를 받아서 저장
        // canExecute는 선택사항 (없으면 항상 실행 가능)
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        // 버튼 활성화 상태가 바뀔 때 WPF가 자동으로 감지하도록 연결
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // 버튼 활성화 여부 반환
        // _canExecute가 null이면 항상 true (항상 실행 가능)
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        // 버튼 클릭 시 실제로 실행되는 메서드
        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
