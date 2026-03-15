using System;
using dotenv.net;

namespace Frends.PDF.Scale.Tests;

public abstract class TestBase
{
    protected TestBase()
    {
        // TODO: Here you can load environment variables used in tests
        DotEnv.Load();
        SecretKey = Environment.GetEnvironmentVariable("FRENDS_SECRET_KEY");
    }

    // TODO: Replace with your secret key or remove if not needed
    protected string SecretKey { get; }
}
