using System;
using LegionMaster.Location.Session.Model;
using LegionMaster.Units.Component;
using UnityEngine.Assertions;

namespace LegionMaster.Location.Session.Service
{
    public class BattleSessionServiceWrapper : IBattleSessionService
    {
        private IBattleSessionService _impl = null;
        private bool _sessionCreated;
        
        public void ChangeImpl(IBattleSessionService impl)
        {
            Assert.IsFalse(_sessionCreated);
            Impl = impl;
            _sessionCreated = true;
        }
        public void StartBattle()
        {
            Impl.StartBattle();
        }
        public BattleSession FinishBattle()
        {
            _sessionCreated = false;
            var rez = Impl.BattleSession;
            Impl.FinishBattle();
            Impl = null;
            return rez;
        }
        public void FinishBattleWithCheats(UnitType winner)
        {
            Impl.FinishBattleWithCheats(winner);
        }
        
        public T GetImpl<T>() where T : class, IBattleSessionService
        {
            if (_impl == null) throw new NullReferenceException($"Wrong IBattleSessionService implementation type. Expected {typeof(T)}, got null. Probably called in wrong battle mode.");
            if (!(Impl is T impl)) throw new Exception($"Wrong IBattleSessionService implementation type. Expected {typeof(T)}, got {_impl.GetType()}. Probably called in wrong battle mode.");
            return impl;
        }
        
        public BattleSession BattleSession => Impl.BattleSession;
        private IBattleSessionService Impl
        {
            get
            {
                if (_impl == null) {
                    throw new NullReferenceException("Wrong IBattleSessionService implementation type. Got NULL. Probably called in wrong battle mode.");
                }
                return _impl;
            }
            set => _impl = value;
        }

     
    }
}