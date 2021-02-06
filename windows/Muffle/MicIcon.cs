using System.Drawing;

namespace Muffle
{
    public abstract class MicIcon
    {
        public abstract Icon NextIcon { get; }
        
        public class Startup : MicIcon
        {
            public override Icon NextIcon => Properties.Resources.microphone_black;
        }

        public class Unmuted : MicIcon
        {
            public override Icon NextIcon => Properties.Resources.microphone_green;
        }

        public class Muted : MicIcon
        {
            public override Icon NextIcon => Properties.Resources.microphone_red;
        }

        public class UnmutedWarning : MicIcon
        {
            private Icon[] _icons =
            {
                Properties.Resources.microphone_yellow,
                Properties.Resources.microphone_green
            };

            private int _currentIndex = 0;

            public override Icon NextIcon
            {
                get
                {
                    _currentIndex++;
                    if (_currentIndex > _icons.GetUpperBound(0))
                        _currentIndex = 0;

                    return _icons[_currentIndex];
                }
            }
        }

        public class MutedWarning : MicIcon
        {
            public Icon[] _icons => new[]
            {
                Properties.Resources.microphone_yellow,
                Properties.Resources.microphone_red
            };

            private int _currentIndex = 0;

            public override Icon NextIcon
            {
                get
                {
                    _currentIndex++;
                    if (_currentIndex > _icons.GetUpperBound(0))
                        _currentIndex = 0;

                    return _icons[_currentIndex];
                }
            }
        }
    }
}