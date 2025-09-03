using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficSignalLight.Dto
{
    public static class SignalProtocolV2
    {
        private static byte ClampU8(int x)
        {
            if (x < 0) return 0;
            if (x > 255) return 255;
            return (byte)x;
        }

        public static byte[] BuildFrameV2(
            int deviceId, int intervalRed, int intervalYellow, int intervalGreen,
            int blinkInterval, bool blinkRed, bool blinkYellow, bool blinkGreen,
            int displayTimer = 0, int crossAsMain = 0, int changeMain = 0)
        {
            byte id = ClampU8(deviceId);
            byte red = ClampU8(intervalRed);
            byte yel = ClampU8(intervalYellow);
            byte grn = ClampU8(intervalGreen);
            // كان: byte blink = ClampU8(blinkInterval);
            byte blink = 0; // <-- force 00 always

            return new byte[]
            {
        0x7B,
        id,
        0x00,
        0x00,
        red,
        0x00,
        yel,
        0x00,
        grn,
        blink,                // دايمًا 00
        (byte)(blinkRed    ? 1 : 0),
        (byte)(blinkYellow ? 1 : 0),
        (byte)(blinkGreen  ? 1 : 0),
        ClampU8(displayTimer),
        ClampU8(crossAsMain),
        ClampU8(changeMain),
        0x00,
        0x7D
            };
        }

        public static string BuildHexV2(
            int deviceId,
            int intervalRed,
            int intervalYellow,
            int intervalGreen,
            int blinkInterval,
            bool blinkRed, bool blinkYellow, bool blinkGreen,
            int displayTimer = 0,
            int crossAsMain = 0,
            int changeMain = 0
        )
        {
            var bytes = BuildFrameV2(deviceId, intervalRed, intervalYellow, intervalGreen,
                                     blinkInterval, blinkRed, blinkYellow, blinkGreen,
                                     displayTimer, crossAsMain, changeMain);

            var sb = new System.Text.StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
                sb.Append(bytes[i].ToString("X2"));
            return sb.ToString();
        }

        public static async Task<bool> SendHttpAsync(string ip, string hex, int timeoutMs = 3000)
        {
            using (var client = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(timeoutMs) })
            {
                try
                {
                    var url = "http://" + ip + "/" + hex;
                    var resp = await client.GetAsync(url);
                    return resp.IsSuccessStatusCode;
                }
                catch { return false; }
            }
        }

        public static async Task<bool> SendTcpAsync(string ip, int port, byte[] payload, int timeoutMs = 2000)
        {
            using (var client = new TcpClient())
            {
                var cts = new CancellationTokenSource(timeoutMs);
                try
                {
                    await client.ConnectAsync(ip, port);
                    using (var s = client.GetStream())
                    {
                        s.WriteTimeout = timeoutMs;
                        await s.WriteAsync(payload, 0, payload.Length, cts.Token);
                        return true;
                    }
                }
                catch { return false; }
            }
        }
    }
}