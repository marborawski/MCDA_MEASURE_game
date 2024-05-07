#pkg install -forge instrument-control
#pkg load instrument-control


clear all

IPAddressSend = '127.0.0.1';
portSend = 55001;

%IPAddressMatlabServer = '0.0.0.0';
%portMatlabServer = 55001;

%tilemap = [   1	1	1	1	1	2	1	1	1	1	1	1	1;
%              1	1	1	1	1	2	1	1	1	1	1	1	1;
%              1	2	2	2	2	2	1	2	2	2	2	2	3;
%              4	2	1	1	1	2	1	2	1	1	1	1	1;
%              1	2	1	1	1	2	2	2	1	1	1	1	1;
%              1	2	1	1	1	2	1	2	1	1	1	1	1;
%              1	2	1	1	1	1	1	2	1	1	1	1	1;
%              1	2	1	1	1	1	1	2	2	2	3	1	1;
%              1	2	1	1	1	1	1	1	1	1	1	1	1;
%              1	4	1	1	1	1	1	1	1	1	1	1	1;
%              1	1	1	1	1	1	1	1	1	1	1	1	1;
%              1	1	1	1	1	1	1	1	1	1	1	1	1];
              
tilemap = [1	1	1	1	1	1	1	1	1	1	1	1	1;
1	1	1	1	1	1	1	1	1	1	1	1	1;
1	2	2	2	2	2	1	2	2	2	2	2	3;
4	2	1	1	1	2	2	2	1	1	1	1	1;
1	1	1	1	1	1	1	1	1	1	1	1	1;
1	1	1	1	1	1	1	1	1	1	1	1	1;
1	1	1	2	2	2	2	1	1	1	1	1	1;
1	2	2	2	1	1	2	2	2	2	3	1	1;
1	2	1	1	1	1	1	1	1	1	1	1	1;
1	4	1	1	1	1	1	1	1	1	1	1	1;
1	1	1	1	1	1	1	1	1	1	1	1	1;
1	1	1	1	1	1	1	1	1	1	1	1	1];

names{1} = 'Ground';
names{2} = 'Water';
names{3} = 'Begin';
names{4} = 'End';

filename = '../Resources/tilemap.xml';

function result = NumberToName(array, names)
  result = cell(size(array));
  for ii = 1:size(array,1)
    for jj = 1:size(array,2)
      result{ii,jj}.name = names{array(ii,jj)};
    end
  end
end

function result = ChangeBeginEnd(array)
  result = array;
  for ii = 1:size(array,1)
    for jj = 1:size(array,2)
      if strcmp(array{ii,jj}.name,'Begin') == 1
        result{ii,jj}.name = 'Water';
        result{ii,jj}.type = array{ii,jj}.name;
      end
      if strcmp(array{ii,jj}.name,'End') == 1
        result{ii,jj}.name = 'Water';
        result{ii,jj}.type = array{ii,jj}.name;
      end
    end
  end
end

function txt = TilemapToXML(tilemap)
  txt = sprintf('\t<Table>');
  for ii = 1:size(tilemap,1)
    txt = [txt sprintf('\t\t<Row>')];
    for jj = 1:size(tilemap,2)
      txt = [txt sprintf('\t\t\t<Cell>')];
      txt = [txt sprintf('<Data')];       
      if isfield(tilemap{ii,jj},'type')
       txt = [txt sprintf(' type="%s"',tilemap{ii,jj}.type)];     
      end
      txt = [txt sprintf('>')];       
      txt = [txt sprintf('%s',tilemap{ii,jj}.name)];      
      txt = [txt sprintf('</Data>')];
      txt = [txt sprintf('</Cell>')];
    end
    txt = [txt sprintf('\t\t</Row>')];
  end
  txt = [txt sprintf('\t</Table>')];

end

function SaveTilemapToXML(filename, txt)
  txtOut = sprintf('<?xml version="1.0" encoding="utf-8"?>\n');
  txtOut = [txtOut sprintf('<Tilemap xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">\n')];
  txtOut = [txtOut, txt];
  txtOut = [txtOut sprintf('</Tilemap>\n')];
  fid=fopen(filename,'wt','n','UTF-8');
  fprintf(fid,'%s',txtOut);

  fclose(fid);
end

function txt = SendData(IPAddressSend,portSend,data,name, args)
  txtOut = sprintf('<?xml version="1.0" encoding="utf-8"?>');
  txtOut = [txtOut sprintf('<Data xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">')];
  if isempty(args)
    txtOut = [txtOut sprintf('<%s>',name)];
  else  
    txtOut = [txtOut sprintf('<%s %s>',name, args)];
  end
  if ~isempty(data)
    txtOut = [txtOut, data];
  end
  txtOut = [txtOut sprintf('</%s>',name)];
  txtOut = [txtOut sprintf('</Data>\n')];
  tcpipClient = tcpclient(IPAddressSend,portSend);
  set(tcpipClient,'Timeout',30);
  writeline(tcpipClient,txtOut,'string');
  pause(0.1);
  txt = read(tcpipClient,100,'uint8');
  while tcpipClient.NumBytesAvailable >= 100
    txt = [txt read(tcpipClient,100,'uint8')];
  end
  if tcpipClient.NumBytesAvailable > 0
    txt = [txt read(tcpipClient,tcpipClient.NumBytesAvailable,'uint8')];    
  end
  clear tcpipClient;
  txt = char(txt);
  if exist('OCTAVE_VERSION', 'builtin') ~= 0;
    txt = strrep(txt,'<?xml version="1.0" encoding="utf-16"?>','<?xml version="1.0"?>');
  else
    txt = replace(txt,'<?xml version="1.0" encoding="utf-16"?>','<?xml version="1.0"?>');
  end
end

function txt = SetEnemies(no,count,speed,startHealth,armour,cost,destroyCoins,coinsToEnd,type, tag)
    txt = sprintf('\t<SetEnemies>');  
    txt = [txt, sprintf('\t\t<Enemy no="%d" count="%d" speed="%d" startHealth="%d" armour="%d" cost="%d" destroyCoins="%d" coinsToEnd="%d" type="%s" tag="%s">',
              no,count,speed,startHealth,armour,cost,destroyCoins,coinsToEnd,type, tag)];  
    txt = [txt, sprintf('\t\t</Enemy>')];
    txt = [txt, sprintf('\t</SetEnemies>')];
end

function txt = SetTowers(no,count,speed,rateofFire,force,bulletStrength,cost,type, tag)
    txt = sprintf('\t<SetTowers>');  
    txt = [txt, sprintf('\t\t<Tower no="%d" count="%d" speed="%d" rateofFire="%d" force="%d" bulletStrength="%d" cost="%d" type="%s" tag="%s">',
              no,count,speed,rateofFire,force,bulletStrength,cost,type, tag)];  
    txt = [txt, sprintf('\t\t</Tower>')];
    txt = [txt, sprintf('\t</SetTowers>')];
end

function txt = AddTower(noTower,x,y)
    txt = sprintf('\t<AddTower no="%d" x="%d" y="%d">',noTower,x,y);  
    txt = [txt, sprintf('\t</AddTower>')];
end

function txt = StartEnemy(noEnemy,beginNo,endNo)
    txt = sprintf('\t<StartEnemy no="%d">',noEnemy);  
    txt = [txt, sprintf('\t\t<Begin no="%d">',beginNo)];  
    txt = [txt, sprintf('\t\t</Begin>')];
    txt = [txt, sprintf('\t\t<End no="%d">',endNo)];  
    txt = [txt, sprintf('\t\t</End>')];
    txt = [txt, sprintf('\t</StartEnemy>')];
end

tilemapNames = NumberToName(tilemap,names);
tilemapNames = ChangeBeginEnd(tilemapNames);

SendData(IPAddressSend,portSend,'','Command','name="Restart"');

pause(1);

txt = TilemapToXML(tilemapNames);
SaveTilemapToXML(filename,txt);
SendData(IPAddressSend,portSend,txt,'Tilemap',[]);

levelData = SendData(IPAddressSend,portSend,[],'Command','name="LevelData"');

txt = SetEnemies(0,-1,2,20,2,30,30,100,'Paper','Enemy');
SendData(IPAddressSend,portSend,txt,'Command','name="SetEnemies"');

txt = SetTowers(0,-2,550,1,1010,5,40,'Tower','Enemy');
SendData(IPAddressSend,portSend,txt,'Command','name="SetTowers"');

choiceOfPathData = SendData(IPAddressSend,portSend,[],'Command','name="GetChoiceOfPathData"');

txt = AddTower(0,5,5);
errorStartEnemy = SendData(IPAddressSend,portSend,txt,'Command','name="AddTower"');

txt = StartEnemy(0,1,1);
errorStartEnemy = SendData(IPAddressSend,portSend,txt,'Command','name="StartEnemy"');






