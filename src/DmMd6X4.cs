// For Basic SIMPL# Classes
// For Basic SIMPL#Pro classes

using System;
using System.Collections.Generic;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro.DM;
using Crestron.SimplSharpPro.DM.Cards;
using Newtonsoft.Json;
using PepperDash.Core;
using PepperDash.Essentials.Core;
using PepperDash.Essentials.Core.Bridges;
using PepperDash.Essentials.Core.Config;
using PepperDash.Essentials.DM;

namespace DmMd6xnEpi
{
    public class DmMd6X4EssentialsDevice : CrestronGenericBridgeableBaseDevice, IDmSwitchWithEndpointOnlineFeedback
    {
        private readonly DmMd6x4 _chassis;
        private readonly Dictionary<uint, IntFeedback> _currentAudioRoutes = new Dictionary<uint, IntFeedback>();
        private readonly Dictionary<uint, IntFeedback> _currentVideoRoutes = new Dictionary<uint, IntFeedback>();
        private readonly Dictionary<uint, StringFeedback> _inputNames = new Dictionary<uint, StringFeedback>();
        private readonly Dictionary<uint, StringFeedback> _outputNames = new Dictionary<uint, StringFeedback>();
        private readonly Dictionary<uint, string> _rxDictionary = new Dictionary<uint, string>();
        private readonly Dictionary<uint, string> _txDictionary = new Dictionary<uint, string>();

        public DmMd6X4EssentialsDevice(string key, string name, GenericBase hardware)
            : base(key, name, hardware)
        {
            var chassis = hardware as DmMd6x4;
            if (chassis == null)
                throw new NullReferenceException(hardware.GetType().Name);

            _chassis = chassis;

            Debug.Console(0,
                this,
                "----------- Device info : NumberOfInputs : {0}, NumberOfOutputs {1}",
                _chassis.NumberOfInputs,
                _chassis.NumberOfOutputs);

            AddPreActivationAction(() =>
                {
                    for (uint x = 1; x <= _chassis.NumberOfOutputs; x++)
                    {
                        Card.DMOCard output;
                        if (!_chassis.Outputs.TryGetValue(x, out output))
                        {
                            Debug.Console(0, this, "----- Output at value {0} doesn't exist", x);
                            continue;
                        }

                        Debug.Console(0,
                            this,
                            "----- Output {0} exists : {1} {2}",
                            x,
                            output.IoType.ToString(),
                            output.NameFeedback.StringValue);

                        _currentVideoRoutes.Add(x, new IntFeedback(
                            () => output.VideoOutFeedback == null ? 0 : (int) output.VideoOutFeedback.Number));


                        _currentAudioRoutes.Add(x, new IntFeedback(
                            () => output.AudioOutFeedback == null ? 0 : (int) output.AudioOutFeedback.Number));

                        _outputNames.Add(x, new StringFeedback(() => output.NameFeedback.StringValue));
                    }

                    for (uint x = 1; x <= _chassis.NumberOfInputs; x++)
                    {
                        DMInput input;
                        if (!_chassis.Inputs.TryGetValue(x, out input))
                        {
                            Debug.Console(0, this, "----- Input at value {0} doesn't exist", x);
                            continue;
                        }

                        _inputNames.Add(x, new StringFeedback(() => input.NameFeedback.StringValue));
                    }

                    _chassis.DMInputChange += (device, args) =>
                        {
                            foreach (var feedback in _currentVideoRoutes.Values)
                                feedback.FireUpdate();
                            foreach (var feedback in _currentAudioRoutes.Values)
                                feedback.FireUpdate();
                        };

                    _chassis.DMOutputChange += (device, args) =>
                        {
                            foreach (var feedback in _currentVideoRoutes.Values)
                                feedback.FireUpdate();
                            foreach (var feedback in _currentAudioRoutes.Values)
                                feedback.FireUpdate();
                        };

                    _chassis.DMSystemChange += (device, args) =>
                        {
                            foreach (var feedback in _inputNames.Values)
                                feedback.FireUpdate();
                            foreach (var feedback in _outputNames.Values)
                                feedback.FireUpdate();
                        };

                    _chassis.OnlineStatusChange += (device, args) =>
                        {
                            foreach (var feedback in _inputNames.Values)
                                feedback.FireUpdate();
                            foreach (var feedback in _outputNames.Values)
                                feedback.FireUpdate();
                        };

                    EnableAudioBreakawayFeedback =
                        new BoolFeedback(() => _chassis.EnableAudioBreakawayFeedback.BoolValue);
                });
        }

        public Switch Chassis
        {
            get { return _chassis; }
        }

        public BoolFeedback EnableAudioBreakawayFeedback { get; private set; }

        public Dictionary<uint, string> RxDictionary
        {
            get { return _rxDictionary; }
        }

        public Dictionary<uint, string> TxDictionary
        {
            get { return _txDictionary; }
        }

        public override void LinkToApi(BasicTriList trilist, uint joinStart, string joinMapKey, EiscApiAdvanced bridge)
        {
            var joinMap = new DmMd6XnJoinMap(joinStart);
            if (bridge != null)
                bridge.AddJoinMap(Key, joinMap);

            IsOnline.LinkInputSig(trilist.BooleanInput[joinMap.DeviceOnline.JoinNumber]);

            //audio breakaway
            EnableAudioBreakawayFeedback.LinkInputSig(trilist.BooleanInput[joinMap.EnableAudioBreakaway.JoinNumber]);
            trilist.SetBoolSigAction(joinMap.EnableAudioBreakaway.JoinNumber,
                b => _chassis.EnableAudioBreakaway.BoolValue = b);

            trilist.SetBoolSigAction(joinMap.VideoEnter.JoinNumber, b => _chassis.VideoEnter.BoolValue = b);
            trilist.SetBoolSigAction(joinMap.AudioEnter.JoinNumber, b => _chassis.AudioEnter.BoolValue = b);

            for (uint x = 0; x < joinMap.VideoRoutes.JoinSpan; x++)
            {
                var index = x + 1;
                IntFeedback feedback;
                if (!_currentVideoRoutes.TryGetValue(index, out feedback))
                    continue;

                var joinActual = x + joinMap.VideoRoutes.JoinNumber;
                feedback.LinkInputSig(trilist.UShortInput[joinActual]);
                trilist.SetUShortSigAction(joinActual,
                    s =>
                        {
                            Debug.Console(1, this, "Routing {0} to {1} | Video", s, index);
                            Card.DMOCard output;
                            if (!_chassis.Outputs.TryGetValue(index, out output))
                                return;

                            if (s == 0)
                            {
                                output.VideoOut = null;
                                return;
                            }

                            DMInput input;
                            if (!_chassis.Inputs.TryGetValue(s, out input))
                                return;

                            output.VideoOut = input;
                        });
            }

            for (uint x = 0; x < joinMap.AudioRoutes.JoinSpan; x++)
            {
                var index = x + 1;
                IntFeedback feedback;
                if (!_currentAudioRoutes.TryGetValue(index, out feedback))
                    continue;

                var joinActual = x + joinMap.AudioRoutes.JoinNumber;
                feedback.LinkInputSig(trilist.UShortInput[joinActual]);
                trilist.SetUShortSigAction(joinActual,
                    s =>
                        {
                            Debug.Console(1, this, "Routing {0} to {1} | Audio", s, index);
                            Card.DMOCard output;
                            if (!_chassis.Outputs.TryGetValue(index, out output))
                                return;

                            if (s == 0)
                            {
                                output.VideoOut = null;
                                return;
                            }

                            DMInput input;
                            if (!_chassis.Inputs.TryGetValue(s, out input))
                                return;

                            output.AudioOut = input;
                        });
            }

            for (uint x = 0; x < joinMap.InputNames.JoinSpan; x++)
            {
                var index = x + 1;
                StringFeedback feedback;
                if (!_inputNames.TryGetValue(index, out feedback))
                    continue;

                var joinActual = x + joinMap.InputNames.JoinNumber;
                feedback.LinkInputSig(trilist.StringInput[joinActual]);
            }

            for (uint x = 0; x < joinMap.OutputNames.JoinSpan; x++)
            {
                var index = x + 1;
                StringFeedback feedback;
                if (!_outputNames.TryGetValue(index, out feedback))
                    continue;

                var joinActual = x + joinMap.OutputNames.JoinNumber;
                feedback.LinkInputSig(trilist.StringInput[joinActual]);
            }
        }

        #region IDmSwitchWithEndpointOnlineFeedback Members

        public Dictionary<uint, BoolFeedback> InputEndpointOnlineFeedbacks
        {
            get { throw new NotImplementedException(); }
        }

        public Dictionary<uint, BoolFeedback> OutputEndpointOnlineFeedbacks
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }

// ReSharper disable once InconsistentNaming
    public class DmMd6x4DeviceFactory : EssentialsPluginDeviceFactory<DmMd6X4EssentialsDevice>
    {
        
        public DmMd6x4DeviceFactory()
        {
            MinimumEssentialsFrameworkVersion = "1.14.3";
            TypeNames = new List<string>
                {
                    "dmMd6x4"
                };
        }

        public override EssentialsDevice BuildDevice(DeviceConfig dc)
        {
            var config = JsonConvert.DeserializeObject<DmMd6x4DeviceConfig>(dc.Properties.ToString());

            var @switch = new DmMd6x4(config.Control.IpIdInt, Global.ControlSystem);
            return new DmMd6X4EssentialsDevice(dc.Key, dc.Name, @switch);
        }
    }

// ReSharper disable once InconsistentNaming
    public class DmMd6x4DeviceConfig
    {
        public EssentialsControlPropertiesConfig Control { get; set; }
    }
}