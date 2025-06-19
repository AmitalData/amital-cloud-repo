using AmitalCloud.Infrastructure.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AmitalCloud.Infrastructure.Application.BaseClasses
{

    public class BaseStateMachineProcess<ProcessState, CommandEnum, RCmmand>
        where ProcessState : struct, IConvertible, IComparable, IFormattable
        where CommandEnum : struct, IConvertible, IComparable, IFormattable
        where RCmmand : RCmmand<ProcessState, CommandEnum>

    {
        public class StateTransition
        {
            readonly ProcessState CurrentState;
            readonly CommandEnum Command;

            public StateTransition(ProcessState currentState, CommandEnum command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(CurrentState, Command);


            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                return other != null &&
                    EqualityComparer<ProcessState>.Default.Equals(this.CurrentState, other.CurrentState) &&
                    EqualityComparer<CommandEnum>.Default.Equals(this.Command, other.Command);
            }
        }

        Dictionary<StateTransition, ProcessState> _Transitions;
        List<RCmmand> _CmmandList;
        public ProcessState CurrentState { get; private set; }
        public CommandEnum CurrentCommand { get; private set; }

        public BaseStateMachineProcess(List<RCmmand> myRCmmands, Dictionary<StateTransition, ProcessState> transitions)
        {
            if (!typeof(ProcessState).IsEnum)
            {
                throw new ArgumentException("ProcessState must be an enum.");
            }
            if (!typeof(CommandEnum).IsEnum)
            {
                throw new ArgumentException("ProcessState must be an enum.");
            }

            _CmmandList = myRCmmands;
            _Transitions = transitions;

            CurrentState = default(ProcessState);
        }

        public ProcessState GetNext(CommandEnum command)
        {

            var stopwatch = Stopwatch.StartNew();
            StateTransition transition = new StateTransition(CurrentState, command);
            ProcessState nextState;
            if (!_Transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
            var rCommand = _CmmandList.FirstOrDefault(e => e.MyCommand.ToString() == command.ToString());
            var toContinue = rCommand.Execute(null);
            LogMessagingUtil.Instance.AppendLine("Current State = " + CurrentState.ToString() + ":Took:" + stopwatch.Elapsed.ToString() + ":toContinue=" + toContinue);
            return nextState;
        }

        public //ProcessState 
            bool
            MoveNext(CommandEnum command, Action<object> OnError = null)
        {

            CurrentCommand = command;
            LogMessagingUtilWR.Instance.AppendLine(command.ToString());

            CurrentState = GetNext(command);
            bool testClient = false;
            if (testClient)
            {
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
            return true;
        }

    }
    // Summary:
    //     Defines a command.

    public interface IMyCommand
    {
        // Summary:
        //     Occurs when changes occur that affect whether or not the command should execute.
        event EventHandler CanExecuteChanged;

        //
        // Summary:
        //     Defines the method to be called when the command is invoked.
        //
        // Parameters:
        //   parameter:
        //     Data used by the command. If the command does not require data to be passed,
        //     this object can be set to null.
        bool Execute(object parameter);
    }
    public class RCmmand<ProcessState, CommandEnum> : IMyCommand //System.Windows.Input.ICommand
        where ProcessState : struct, IConvertible, IComparable, IFormattable
        where CommandEnum : struct, IConvertible, IComparable, IFormattable
    {
        public CommandEnum MyCommand { get; private set; }

        readonly Func<object, bool> _func;
#if false
        readonly Action<object> _execute;
        public RCmmand(CommandEnum command, Action<object> execute)
        {

            if (!typeof(ProcessState).IsEnum)
            {
                throw new ArgumentException("ProcessState must be an enum.");
            }
            if (!typeof(CommandEnum).IsEnum)
            {
                throw new ArgumentException("ProcessState must be an enum.");
            }

            MyCommand = command;
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;

        }
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

#endif

        public RCmmand(CommandEnum command, Func<object, bool> func)
        {
            MyCommand = command;
            if (func == null)
                throw new ArgumentNullException("execute");

            _func = func;

        }

        public bool Execute(object parameter)
        {
            var res = _func(parameter);
            return res;
        }

        public event EventHandler CanExecuteChanged;
    }

}
