using PepperDash.Essentials.Core;

namespace DmMd6xnEpi
{
    public class DmMd6XnJoinMap : JoinMapBaseAdvanced
    {
        [JoinName("DeviceOnline")]
        public JoinDataComplete DeviceOnline = new JoinDataComplete(
            new JoinData { JoinNumber = 1, JoinSpan = 1 },
            new JoinMetadata
                {
                    Description = "DM Chassis online",
                    JoinCapabilities = eJoinCapabilities.ToSIMPL,
                    JoinType = eJoinType.Digital
                });

        [JoinName("VideoEnter")]
        public JoinDataComplete VideoEnter = new JoinDataComplete(
            new JoinData { JoinNumber = 1, JoinSpan = 1 },
            new JoinMetadata
                {
                    Description = "DM Chassis video enter",
                    JoinCapabilities = eJoinCapabilities.FromSIMPL,
                    JoinType = eJoinType.Digital
                });

        [JoinName("AudioEnter")]
        public JoinDataComplete AudioEnter = new JoinDataComplete(
            new JoinData { JoinNumber = 2, JoinSpan = 1 },
            new JoinMetadata
                {
                    Description = "DM Chassis audio enter",
                    JoinCapabilities = eJoinCapabilities.FromSIMPL,
                    JoinType = eJoinType.Digital
                });

        [JoinName("EnableAudioBreakaway")]
        public JoinDataComplete EnableAudioBreakaway = new JoinDataComplete(
            new JoinData { JoinNumber = 3, JoinSpan = 1 },
            new JoinMetadata
                {
                    Description = "DM Chassis enable audio breakaway routing",
                    JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                    JoinType = eJoinType.Digital
                });

        [JoinName("VideoRoutes")]
        public JoinDataComplete VideoRoutes = new JoinDataComplete(
            new JoinData { JoinNumber = 11, JoinSpan = 10 },
            new JoinMetadata
                {
                    Description = "DM Chassis video routes",
                    JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                    JoinType = eJoinType.Analog
                });

        [JoinName("AudioRoutes")]
        public JoinDataComplete AudioRoutes = new JoinDataComplete(
            new JoinData { JoinNumber = 21, JoinSpan = 10 },
            new JoinMetadata
                {
                    Description = "DM Chassis video routes",
                    JoinCapabilities = eJoinCapabilities.ToFromSIMPL,
                    JoinType = eJoinType.Analog
                });

        [JoinName("InputNames")]
        public JoinDataComplete InputNames = new JoinDataComplete(
            new JoinData { JoinNumber = 1, JoinSpan = 10 },
            new JoinMetadata
            {
                Description = "DM Chassis input names",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial
            });

        [JoinName("OutputNames")]
        public JoinDataComplete OutputNames = new JoinDataComplete(
            new JoinData { JoinNumber = 11, JoinSpan = 10 },
            new JoinMetadata
            {
                Description = "DM Chassis output names",
                JoinCapabilities = eJoinCapabilities.ToSIMPL,
                JoinType = eJoinType.Serial
            });

        public DmMd6XnJoinMap(uint joinStart) : base(joinStart, typeof(DmMd6XnJoinMap)) { }
    }
}