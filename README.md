NBitcoin.PaymentServer
======================

A bitcoin payment processing server based on NBitcoin.

This server allows an organisation to directly receive bitcoin payments (donations, 
purchases, etc) without having to use a payment processing gateway.

It uses BIP-0032 hierarchical deterministic (HD) addresses to track each payment, 
with only the public generator key needing to be loaded onto the server. This means 
that the private key can be kept secure.

Currently, the payment server utilises a full bitcoind server node to verify payments 
have been made.

The system is similar to the Straight Server (Mycellium Gear), but .NET based.


Example Site
------------

### Demo site

Using Main network; but verifications may not work as my bitcoind node can 
sometimes be a bit behind.

  https://tanstaafl-cafe.azurewebsites.net/

### Test site

Using TestNet. May not always work, as this is where I deploy work in progress
for testing, but my TestNet server is more likely to be up to date and work.

  https://tanstaafl-cafe-ci.azurewebsites.net/


Limitations
-----------

This is only the initial proof of concept.

It does work end-to-end, but only for basic scenarios (donation).

* Currency conversion is not implemented; BTC works and there is a simple converter for mBTC; 
  AUD and USD are hard coded.

* There is no callback/confirmation function to the originating website, so it can only 
  really be used for one-way donations, no actual purchases.

* Also values (e.g. payment amount) is passed on the query string with no signing or other
  protection, so can be easily changed (which doesn't really matter for donations).

* There is no flooding or DoS attach protection.

* No UI for editing / managing gateways or past transactions; everything needs to be done
  directly in the database.

* No QR code generation, and the UI is very basic.

* Verification is based on a full bitcoind node, which is difficult to run (especially if
  it falls behind). A simpler solution (maybe SPV based) would be good, or maybe the 
  NBitcoin Indexer.

* Alternative verifiers, e.g. blockchaininfo, are also possible.

* Doesn't have any unit tests yet.


Security
--------

The main system is designed to be safe by only storing the master public key, which
if compromised will reveal all the related adddresses, but not allow access to any 
of the actual bitcoins.

Likewise, addresses registered for verification are stored as watch only, with only
the address available (only the public addresses can be generated from the public key).

The source code, however, contains a bunch of compromised TestNet addresses.

These include the RPC details for my TestNet node. This isn't really an issue,
because (as mentioned above), only addresses (and no secrets) are stored; besides,
it is TestNet, so not an issue.

In fact, the TestNet secrets are also in the source code comments, so if someone 
really wants to mess with my TestNet accounts it will be annoying, but I can always
just get more from a faucet. 


Development & build
-------------------

Written in .NET Core, using Visual Studio.

The main projects are:

* ExampleMerchant.TanstaaflCafe - example merchant web site, links to the payment server site
* NBitcoin.PaymentServer - main library containing all the bulk of code
* NBitcoin.PaymentServer.Db - console application that uses DbUp migrations to build the database (needed before the server will work); run it locally for a development database
* NBitcoin.PaymentServer.Web - web interface to the PaymentServer library (a thin, simple layer)

Build these, and run the database utility to generate a local database. You will need to configure the system to point to an accessible TestNet bitcoind server, and then then example merchant and payment server sites should work locally.

Some additional utilities that may be useful during development:

* NBitcoin.PaymentServer.TestTool - console app for making manual payments (from TestNet 'customer' addresses to the payment server address); you could also just use a wallet to do this
* NBitcoin.PaymentServer.Utility - console utility that will check payments and also collect (sweep) payments from an address to another address (also see below for a way to set up Electrum to do this)

The project has a build and release pipeline set up in Visual Studio Team System, that deploys to the TANSTAAFL Café test & demo sites (in Azure). The pipeline handles things like version numbering (using Git Version).


To configure the public key using a HD wallet
---------------------------------------------

The example uses Electrum, but any wallet that supports standard HD should work similar. 

Electrum only has direct support for a single level of HD addresses (and you would probably want a hardened address anyway), so the easiest solution is to create a brand new wallet with a master key for your payments (and you still need to then derive the right public key for the server -- see below).

You can then regularly collect the money in this wallet and send it to your main address or to a currency conversion service.

1. File -> New/Restore

2. Enter the name, e.g. wallet_tanstaaflcafe

3. Select Create a new wallet, kind Standard wallet

4. Keep a note of the seed, in case you ever lose your wallet

5. Re-enter the seed, then enter a password

Electrum uses 'm/0/*' for wallet addresses and 'm/1/*' for change addresses, so to get the same addresses as displayed in the wallet we first need to derive the 'm/0' key.

6. When the new wallet appears, select Wallet -> Master Public Keys

7. Use the utility program from the project to derive the m/0 key. The output will show both the derived key and derived address; for this level you want the key value.

    dotnet NBitcoin.PaymentServer.Utility.dll -o derive -m '<master public key>' -i 'm/0'

8. The derived key value from the above is what you need to configure the payment server with, but first, to check it works, ensure the address output matches the first address in the wallet:

    dotnet NBitcoin.PaymentServer.Utility.dll -o derive -m '<derived key>' -i 'm/0'

(You can also get this by deriving 'm/0/0' from the original master key)

9. The derived key needs to be stored in the server, to generate the same addresses as the wallet

Currently this needs to be done directly in the database, via SQL Server Management Studio.

Put your values into the following SQL statement, and run it in the database. This will replace gateway #1 with your own custom values. You need to do this before running any transactions (otherwise there will be references to the ID).

    DELETE FROM [dbo].[GatewayKeyIndexes];
    DELETE FROM [dbo].[Gateways];
    SET IDENTITY_INSERT [dbo].[Gateways] ON;
    INSERT INTO [dbo].[Gateways] (Id, GatewayNumber, Name, ExtPubKey)
        VALUES (NEWID(), 1, '<store name>', '<derived key>');
    INSERT INTO [dbo].[GatewayKeyIndexes] (GatewayId, LastKeyIndex)
        VALUES ((SELECT Id FROM [dbo].[Gateways] WHERE GatewayNumber = 1), 0);
    SET IDENTITY_INSERT [dbo].[Gateways] OFF

Once set up, the server should generate the same sequence of payment addresses as Electrum, so any transactions can be managed in Electrum (which has the private key). The server can generate addresses, because it has the public key, but can't do anything except watch them.

