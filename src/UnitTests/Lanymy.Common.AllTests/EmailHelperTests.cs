using System.Linq;
using Lanymy.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lanymy.Common.AllTests
{
    [TestClass]
    public class EmailHelperTests
    {
        [TestMethod]
        public void EmailHelper_SendEmailWithResult_WhenBatchSendRecipientsFail_ShouldReportEachRecipientFailure()
        {
            var result = EmailHelper.SendEmailWithResult(
                "127.0.0.1",
                "sender@example.com",
                "password",
                " user1@example.com , user2@example.com ",
                "subject",
                "body",
                port: 1,
                isUnpackSendMail: false,
                senderDisplayName: "Lanymy Sender");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual(2, result.RequestedRecipientCount);
            CollectionAssert.AreEqual(new[] { "user1@example.com", "user2@example.com" }, result.RequestedRecipientAddresses.ToList());
            Assert.AreEqual(2, result.RecipientResults.Count);
            Assert.AreEqual(0, result.SuccessCount);
            Assert.AreEqual(2, result.FailureCount);
            Assert.IsTrue(result.RecipientResults.All(item => !item.IsSuccess && item.Exception != null));
            Assert.AreEqual("utf-8", result.BodyEncodingName);
            Assert.AreEqual("Lanymy Sender", result.SenderDisplayName);
            Assert.AreEqual(1, result.Port);
            Assert.IsFalse(result.EnableSsl);
            Assert.IsFalse(result.IsUnpackSendMail);
        }

        [TestMethod]
        public void EmailHelper_SendEmailWithResult_WhenUnpackSendAllRecipientsFail_ShouldReportFailureDetails()
        {
            var result = EmailHelper.SendEmailWithResult(
                "127.0.0.1",
                "sender@example.com",
                "password",
                "user1@example.com,user2@example.com",
                "subject",
                "body",
                port: 1,
                isUnpackSendMail: true);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
            Assert.AreEqual(2, result.RequestedRecipientCount);
            Assert.AreEqual(2, result.RecipientResults.Count);
            Assert.AreEqual(0, result.SuccessCount);
            Assert.AreEqual(2, result.FailureCount);
            Assert.IsTrue(result.RecipientResults.All(item => !item.IsSuccess && item.Exception != null));
            Assert.IsTrue(result.IsUnpackSendMail);
        }

        [TestMethod]
        public void EmailHelper_SendEmailWithResult_WhenRecipientsAreBlankAfterNormalization_ShouldReturnArgumentFailure()
        {
            var result = EmailHelper.SendEmailWithResult(
                "127.0.0.1",
                "sender@example.com",
                "password",
                " , , ",
                "subject",
                null,
                port: 1);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsInstanceOfType(result.Exception, typeof(System.ArgumentException));
            Assert.AreEqual(0, result.RequestedRecipientCount);
            Assert.AreEqual(0, result.RecipientResults.Count);
            Assert.AreEqual("utf-8", result.BodyEncodingName);
        }

        [TestMethod]
        public void EmailHelper_SendEmail_WhenUnpackSendAllRecipientsFail_ShouldKeepCompatibilityFailureSemantics()
        {
            var result = EmailHelper.SendEmail(
                "127.0.0.1",
                "sender@example.com",
                "password",
                "user1@example.com,user2@example.com",
                "subject",
                "body",
                port: 1,
                isUnpackSendMail: true);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNotNull(result.Exception);
        }
    }
}
