using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace TRGE.View.Utils
{
    //https://stackoverflow.com/questions/29905192/soundplayer-stop-does-not-stop-sound-playback
    public class AudioPlayer : SoundPlayer
    {
        private readonly CancellationTokenSource _cancelSource;
        private readonly CancellationToken _cancelToken;

        private readonly double _duration;
        private bool _playing;

        public event EventHandler AudioStarted;
        public event EventHandler AudioCompleted;

        public AudioPlayer(byte[] trackData)
            : base(new MemoryStream(trackData))
        {
            _cancelSource = new CancellationTokenSource();
            _cancelToken = _cancelSource.Token;

            if (trackData.Length < 32)
            {
                throw new ArgumentException();
            }

            _duration = 1000 * (double)(trackData.Length - 8) / BitConverter.ToInt32(trackData, 28);

            _playing = false;
        }

        public void PlayAudio()
        {
            if (_playing)
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    _playing = true;
                    AudioStarted?.Invoke(this, EventArgs.Empty);
                    DateTime endTime = DateTime.Now.AddMilliseconds(_duration);
                    Play();
                    while (DateTime.Now < endTime)
                    {
                        _cancelToken.ThrowIfCancellationRequested();
                        Task.Delay(10).Wait();
                    }
                }
                catch (OperationCanceledException)
                {
                    Stop();
                }
                finally
                {
                    _playing = false;
                    AudioCompleted?.Invoke(this, EventArgs.Empty);
                }
            }, _cancelToken);
        }

        public void StopAudio()
        {
            if (_playing)
            {
                _cancelSource.Cancel();
                while (_playing)
                {
                    Thread.Sleep(10);
                }
            }
        }
    }
}