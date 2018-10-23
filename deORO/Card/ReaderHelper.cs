using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using deORO.Helpers;
using Microsoft.Practices.Composite.Events;
using deORODataAccessApp;
using deORODataAccessApp.DataAccess;
using deORODataAccessApp.Models;
//using HidGlobal.OK.Readers;
//using HidGlobal.OK.Readers.AViatoR.Components;
//using HidGlobal.OK.Readers.Components;
//using HidGlobal.OK.Readers.SecureSession;

namespace deORO.Card
{
    //class ReaderHelper
    //{
    //    public static void ConnectToReaderWithCard(ISmartCardReader smartCardReader)
    //    {
    //        try
    //        {
    //            ReaderState state = ContextHandler.Instance.GetReaderState(smartCardReader.PcscReaderName, ReaderStates.Unaware);
    //            if (state.AtrLength > 0)
    //            {
    //                smartCardReader.Connect(ReaderSharingMode.Shared, Protocol.Any);
    //            }
    //        }
    //            catch (Exception ex)
    //            {
    //                aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
    //                {
    //                    DeviceType = "HIDCard",
    //                    Message = ex.Message + " " + ex.StackTrace
    //                });
    //            }
    //    }
    //    public static void ConnectToReader(ISmartCardReader smartCardReader)
    //    {
    //        try
    //        {
    //            ReaderState state = ContextHandler.Instance.GetReaderState(smartCardReader.PcscReaderName, ReaderStates.Unaware);
    //            if (state.AtrLength > 0)
    //            {
    //                smartCardReader.Connect(ReaderSharingMode.Shared, Protocol.Any);
    //            }
    //            else
    //            {
    //                smartCardReader.ConnectDirect();
    //            }
    //        }
    //            catch (Exception ex)
    //            {
    //                aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
    //                {
    //                    DeviceType = "HIDCard",
    //                    Message = ex.Message + " " + ex.StackTrace
    //                });
    //            }
    //    }
    //    public static IReadOnlyList<byte> SendCommand(ISmartCardReader smartCardReader, IReadOnlyList<byte> command)
    //    {
    //        return smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
    //    }
    //    public static string SendCommand(ISmartCardReader smartCardReader, string command)
    //    {
    //        return smartCardReader.ConnectionMode != ReaderSharingMode.Direct ? smartCardReader.Transmit(command) : smartCardReader.Control(ReaderControlCode.IOCTL_CCID_ESCAPE, command);
    //    }
    //    public static void GeneralAuthenticateiClass(ISmartCardReader smartCardReader, string description, BookNumber book, PageNumber page, GeneralAuthenticateCommand.ImplicitSelection implicitSelection, GeneralAuthenticateCommand.iClassKeyType keyType, byte keySlot)
    //    {
    //        var generalAuthenticateCommand = new Readers.AViatoR.Components.GeneralAuthenticateCommand();

    //        string input =
    //            generalAuthenticateCommand.GetiClassApdu(book, page, implicitSelection, keyType, keySlot);
    //        string output = smartCardReader.Transmit(input);

    //        ConsoleWriter.Instance.PrintCommand(description + keySlot.ToString("X2"), input, output);
    //    }
    //    public static void GeneralAuthenticateiClass(ISecureChannel secureChannel, string description, BookNumber book, PageNumber page, GeneralAuthenticateCommand.ImplicitSelection implicitSelection, GeneralAuthenticateCommand.iClassKeyType keyType, byte keySlot)
    //    {
    //        var generalAuthenticateCommand = new Readers.AViatoR.Components.GeneralAuthenticateCommand();

    //        string input =
    //            generalAuthenticateCommand.GetiClassApdu(book, page, implicitSelection, keyType, keySlot);
    //        string output = secureChannel.SendCommand(input);

    //        ConsoleWriter.Instance.PrintCommand(description + keySlot.ToString("X2"), input, output);
    //    }
    //    public static void ReadBinaryiClassCommand(ISmartCardReader smartCardReader, string description, ReadBinaryCommand.ReadOption readOption, byte blockNumber, byte expectedlength, BookNumber book = BookNumber.Book0, PageNumber page = PageNumber.Page0)
    //    {
    //        var readBinaryCommand = new Readers.AViatoR.Components.ReadBinaryCommand();

    //        string input = readBinaryCommand.GetiClassReadApdu(readOption, blockNumber, expectedlength, book, page);
    //        string output = smartCardReader.Transmit(input);

    //        ConsoleWriter.Instance.PrintCommand(description + "0x" + blockNumber.ToString("X2"), input, output);
    //    }
    //    public static void ReadBinaryiClassCommand(ISecureChannel secureChannel, string description, ReadBinaryCommand.ReadOption readOption, byte blockNumber, byte expectedlength, BookNumber book = BookNumber.Book0, PageNumber page = PageNumber.Page0)
    //    {
    //        var readBinaryCommand = new Readers.AViatoR.Components.ReadBinaryCommand();

    //        string input = readBinaryCommand.GetiClassReadApdu(readOption, blockNumber, expectedlength, book, page);
    //        string output = secureChannel.SendCommand(input);

    //        ConsoleWriter.Instance.PrintCommand(description + "0x" + blockNumber.ToString("X2"), input, output);
    //    }
    //    public static void UpdateBinaryCommand(ISmartCardReader smartCardReader, string description, UpdateBinaryCommand.Type type, byte blockNumber, string data)
    //    {
    //        var updateBinaryCommand = new UpdateBinaryCommand();

    //        string input = updateBinaryCommand.GetApdu(Readers.AViatoR.Components.UpdateBinaryCommand.Type.Plain,
    //            blockNumber, data);
    //        string output = smartCardReader.Transmit(input);

    //        ConsoleWriter.Instance.PrintCommand(description + "0x" + blockNumber.ToString("X2"), input, output);
    //    }
    //    public static void UpdateBinaryCommand(ISecureChannel secureChannel, string description, UpdateBinaryCommand.Type type, byte blockNumber, string data)
    //    {
    //        var updateBinaryCommand = new UpdateBinaryCommand();

    //        string input = updateBinaryCommand.GetApdu(Readers.AViatoR.Components.UpdateBinaryCommand.Type.Plain,
    //            blockNumber, data);
    //        string output = secureChannel.SendCommand(input);

    //        ConsoleWriter.Instance.PrintCommand(description + "0x" + blockNumber.ToString("X2"), input, output);
    //    }
    //    public static void ReadBinaryMifareCommand(ISmartCardReader smartCardReader, string description, byte blockNumber, byte expectedlength)
    //    {
    //        var readBinaryCommand = new Readers.AViatoR.Components.ReadBinaryCommand();

    //        string input = readBinaryCommand.GetMifareReadApdu(blockNumber, expectedlength);
    //        string output = smartCardReader.Transmit(input);

    //        ConsoleWriter.Instance.PrintCommand(description + "0x" + blockNumber.ToString("X2"), input, output);
    //    }
    //    public static void GeneralAuthenticateMifare(ISmartCardReader smartCardReader, string description, byte blockNumber, GeneralAuthenticateCommand.MifareKeyType keyType, byte keySlot)
    //    {
    //        var generalAuthenticateCommand = new Readers.AViatoR.Components.GeneralAuthenticateCommand();

    //        string input =
    //            generalAuthenticateCommand.GetMifareApdu(blockNumber, keyType, keySlot);
    //        string output = smartCardReader.Transmit(input);

    //        ConsoleWriter.Instance.PrintCommand(description + keySlot.ToString("X2"), input, output);
    //    }
    //    public static void GetDataCommand(ISmartCardReader smartCardReader, string description, GetDataCommand.Type type)
    //    {
    //        var getData = new GetDataCommand();

    //        ConsoleWriter.Instance.PrintMessage(description);

    //        string input = getData.GetApdu(type);
    //        string output = smartCardReader.Transmit(input);

    //        //ConsoleWriter.Instance.PrintCommand(string.Empty, input, output,
    //        //    $"Data: {output.Substring(0, output.Length - 4)}");
    //    }
    //    public static string GetSerialNumber(string readerName)
    //    {
    //        string readerSerialNumber;

    //        using (var reader = new SmartCardReader(readerName))
    //        {
    //            var getSerialNumberCommand = new Readers.AViatoR.Components.SerialNumber();

    //            ConnectToReader(reader);

    //            try
    //            {
    //                var response = SendCommand(reader, getSerialNumberCommand.GetApdu);
    //                readerSerialNumber = getSerialNumberCommand.TranslateResponse(response);
    //            }
    //            catch (Exception ex)
    //            {
    //                aggregator.GetEvent<EventAggregation.DeviceInitFailEvent>().Publish(new DeviceFailEventArgs()
    //                {
    //                    DeviceType = "HIDCard",
    //                    Message = ex.Message + " " + ex.StackTrace
    //                });
    //            }
    //            finally
    //            {
    //                if (reader.IsConnected)
    //                    reader.Disconnect(CardDisposition.Unpower);
    //            }
    //        }
    //        return readerSerialNumber;
    //    }
    //}
}
