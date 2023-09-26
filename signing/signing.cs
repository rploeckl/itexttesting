using iText.Bouncycastleconnector;
using iText.Commons.Bouncycastle;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Commons.Bouncycastle.Openssl;
using iText.Kernel.Pdf;
using iText.Signatures;
// using itext8_project.s81866;
using Org.BouncyCastle.Crypto;

namespace itext8_project.ReleaseExample;

public class SignTest
{
    public void FipsExample(string inFile, String outFile, string pemPath, string pw) {
        CryptoServicesRegistrar.SetApprovedOnlyMode(true);
        var chain = PemFileHelper.ReadFirstChain(pemPath);
        var privateKey = PemFileHelper.ReadFirstKey(pemPath, pw.ToCharArray());
        var pk = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);
        var signer = new PdfSigner(new PdfReader(inFile), new FileStream(outFile, FileMode.Create),
            new StampingProperties().UseAppendMode());


        signer.SignDetached(pk, chain, null, null, null, 0, PdfSigner.CryptoStandard.CMS);
    }
    
    
    class PemFileHelper {
        private static readonly IBouncyCastleFactory FACTORY = BouncyCastleFactoryCreator.GetFactory();
        private PemFileHelper() {
            // Empty constructor.
        }
        public static IX509Certificate[] ReadFirstChain(String pemFileName) {
            return ReadCertificates(pemFileName).ToArray(new IX509Certificate[0]);
        }
        public static IPrivateKey ReadFirstKey(String pemFileName, char[] keyPass) {
            return ReadPrivateKey(pemFileName, keyPass);
        }
        public static List InitStore(String pemFileName) {
            IX509Certificate[] chain = ReadFirstChain(pemFileName);
            return chain.Length > 0 ? new List { chain[0] } : chain.ToList();
        }
        private static IList ReadCertificates(String pemFileName) {
            using (TextReader file = new StreamReader(pemFileName)) {
                IPemReader parser = FACTORY.CreatePEMParser(file, null);
                Object readObject = parser.ReadObject();
                IList certificates = new List();
                while (readObject != null) {
                    if (readObject is IX509Certificate) {
                        certificates.Add((IX509Certificate)readObject);
                    }
                    readObject = parser.ReadObject();
                }
                return certificates;
            }
        }
        private static IPrivateKey ReadPrivateKey(String pemFileName, char[] keyPass) {
            using (TextReader file = new StreamReader(pemFileName)) {
                IPemReader parser = FACTORY.CreatePEMParser(file, keyPass);
                Object readObject = parser.ReadObject();
                while (!(readObject is IPrivateKey) && readObject != null) {
                    readObject = parser.ReadObject();
                }
                return (IPrivateKey)readObject;
            }
        }
    }
}