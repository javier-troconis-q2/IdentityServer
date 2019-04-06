using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServer.AzureFunction
{
	public class AppSettings
	{
		public ApiResource[] ApiResources { get; set; }
		public Client[] Clients { get; set; }
		public string SigningCertificatePath { get; set; }
		public string SigningCertificatePassword { get; set; }

		public class Client
		{
			public string ClientId { get; set; }
			public string[] ClientSecrets { get; set; }
			public string[] AllowedScopes { get; set; }
		}

		public class ApiResource
		{
			public string Name { get; set; }
			public string[] Scopes { get; set; }
		}
	}
}
