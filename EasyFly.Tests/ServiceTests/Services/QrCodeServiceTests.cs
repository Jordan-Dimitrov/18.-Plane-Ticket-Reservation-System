using NUnit.Framework;
using System;
using System.Linq;
using EasyFly.Infrastructure.Services;

namespace EasyFly.Tests.ServiceTests.Services
{
    [TestFixture]
    public class QrCodeServiceTests
    {
        private QrCodeService _qrCodeService;

        [SetUp]
        public void Setup()
        {
            _qrCodeService = new QrCodeService();
        }

        [Test]
        public void GenerateQRCode_WithValidInput_ReturnsNonEmptyByteArray()
        {
            string inputText = "Hello, World!";
            int size = 300;
            byte[] result = _qrCodeService.GenerateQRCode(inputText, size);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [Test]
        public void GenerateQRCode_ReturnedImage_HasPngSignature()
        {
            string inputText = "Test QR Code";
            int size = 300;
            byte[] result = _qrCodeService.GenerateQRCode(inputText, size);
            byte[] pngHeader = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            Assert.IsTrue(result.Length >= pngHeader.Length);
            Assert.IsTrue(result.Take(pngHeader.Length).SequenceEqual(pngHeader));
        }

        [Test]
        public void GenerateQRCode_SameInputAndSize_ReturnsConsistentOutput()
        {
            string inputText = "Consistent QR Code";
            int size = 300;
            byte[] firstResult = _qrCodeService.GenerateQRCode(inputText, size);
            byte[] secondResult = _qrCodeService.GenerateQRCode(inputText, size);
            Assert.IsTrue(firstResult.SequenceEqual(secondResult));
        }

        [Test]
        public void GenerateQRCode_DifferentSize_ReturnsDifferentOutputLength()
        {
            string inputText = "Different sizes test";
            int sizeSmall = 200;
            int sizeLarge = 400;
            byte[] smallResult = _qrCodeService.GenerateQRCode(inputText, sizeSmall);
            byte[] largeResult = _qrCodeService.GenerateQRCode(inputText, sizeLarge);
            Assert.AreNotEqual(smallResult.Length, largeResult.Length);
        }
    }
}
