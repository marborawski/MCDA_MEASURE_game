clear all

nazwa_pliku = 'dane_do_obliczen_2016';
plik_wynikowy = 'WASPAS_wyniki_2016';

liczbaKlas = 4;

t = 1;

%  lata{t} = '2005';t = t + 1;
%  lata{t} = '2006';t = t + 1;
%  lata{t} = '2007';t = t + 1;
%  lata{t} = '2008';t = t + 1;
%  lata{t} = '2009';t = t + 1;
%  lata{t} = '2010';t = t + 1;
%  lata{t} = '2011';t = t + 1;
%  lata{t} = '2012';t = t + 1;
%  lata{t} = '2013';t = t + 1;
% lata{t} = '2014';t = t + 1;
%  lata{t} = '2015';t = t + 1;
lata{t} = '2016';t = t + 1;


% lambda=1;
% s_lambda = -0.15*lambda+0.3;

[a b param] = xlsread(nazwa_pliku,'parametry');

for ii = 2:size(param,1)
    zmienne{ii-1} = param{ii,1};
    wagi{ii-1} = param{ii,2};
    charakter{ii-1} = param{ii,3};
end

wagi = convert(wagi);

[data nazwy_wariantow_decyzyjnych code] = readData(nazwa_pliku,lata);

saveDataXLS('Dane',data,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],[]);

[data color] = interpolacja(data);

saveDataXLS('Interpolacja',data,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

wagi2 = wagi/sum(wagi);

tmp = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    tmp(ii) = sum(res);
end

weight3 = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    weight3(ii) = std(res)/mean(res);
end

weight3 = weight3/sum(weight3);

wagi2 = weight3;


tmp = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    if strcmp(charakter(ii),'s') == 1
        tmp(ii) = max(res);
    else
        tmp(ii) = min(res);
    end
end

dane_norm = cell(1,length(data));
for k = 1:length(data)
    dane_norm{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        if strcmp(charakter(ii),'s') == 1
            dane_norm{k}(:,ii) = data{k}(:,ii)/tmp(ii);
        else
            dane_norm{k}(:,ii) = tmp(ii)./data{k}(:,ii);
        end
    end
end

saveDataXLS('Normowanie',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);


Q1 = cell(1,length(dane_norm));
Q2 = cell(1,length(dane_norm));
Q = cell(1,length(dane_norm));
lambda = cell(1,length(dane_norm));
for k = 1:length(dane_norm)
    Q1{k} = zeros(size(dane_norm{1},1),1);
    Q2{k} = zeros(size(dane_norm{1},1),1);
    Q{k} = zeros(size(dane_norm{1},1),1);
    for ii = 1:size(dane_norm{1},1)
        Q1{k}(ii) = sum(dane_norm{k}(ii,:).*wagi2);
        il = 1;
        for kk = 1:length(dane_norm{k}(ii,:))
            il = il*(dane_norm{k}(ii,kk)^wagi2(kk));
        end
        Q2{k}(ii) = il;
        lambda{k} = std(Q2{k})/(std(Q1{k}) + std(Q2{k}));
    end
    Q{k}(:,1) = lambda{k}*Q1{k} + (1 - lambda{k})*Q2{k};
   
    Q{k}(:,2) = klasyfikacja(Q{k}(:,1),liczbaKlas);
end


tmp = cell(1,1);
tmp{1} = 'Wartosc miary';
tmp{2} = 'Klasa';


saveDataXLS(plik_wynikowy,Q,lata,tmp,nazwy_wariantow_decyzyjnych,code,[],[]);

