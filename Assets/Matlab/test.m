clear all

%machine



data = fileread('towers.xml');

data = parseXML(data);