using System;

namespace Clusters;

public static class StringHelper
{
    public const string AllSymbolsInput = "ABCDEFGHIJKLMNOPQRSTUVWXYZ abcdefghijklmnopqrstuvwxyz" +
    "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ абвгдеёжзийклмнопрстуфхцчшщъыьэюя" +
    "0123456789" +
    "!@#$%^&*()_+-=[]{}|;:'\",.<>/?" +
    "\n\r\t\0";

    public const string SampleMessage = "Исполняемый файл event_sender-100.392692.Debug-0.rockspec' " +
        "запущен из неожиданного каталога /tmp/luarocks_event_sender-100.392692.Debug-0-4030925' && " +
        "cp '/tmp/luarocks_event_sender-100.392692.Debug-0-4030925/event_sender-100.392692.Debug-0.rockspec' " +
        "'/opt/ptaf/toolchain/usr/lib/luarocks/rocks-5.1/event_sender/100.392692.Debug-0/ на узле localhost";
}
