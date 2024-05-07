clear all

nazwa_pliku = 'dane_do_obliczen_2016.xlsx';
%plik_wynikowy = 'TOPSIS_wyniki_2016';
plik_wynikowy = 'TOPSIS_wyniki';

liczbaKlas = 4;

t = 1;

lata{t} = '2005';t = t + 1;
lata{t} = '2006';t = t + 1;
lata{t} = '2007';t = t + 1;
lata{t} = '2008';t = t + 1;
lata{t} = '2009';t = t + 1;
lata{t} = '2010';t = t + 1;
lata{t} = '2011';t = t + 1;
lata{t} = '2012';t = t + 1;
lata{t} = '2013';t = t + 1;
lata{t} = '2014';t = t + 1;
lata{t} = '2015';t = t + 1;
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

for kk = 1:length(data)

  tmp{kk} = zeros(1,size(data{1},2));
  for ii = 1:size(data{1},2)
      res = [];
      res = data{kk}(:,ii);
%      for k = 1:length(data)
%          res = [res; data{k}(:,ii)];
%      end
      tmp{kk}(ii) = sum(res);
  end
end

tmp_dane_entropy_norm = cell(1,length(data));
for k = 1:length(data)
    tmp_dane_entropy_norm{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        tmp_dane_entropy_norm{k}(:,ii) = data{k}(:,ii)/tmp{k}(ii);
    end
end

weight_entropy = zeros(1,size(tmp_dane_entropy_norm{1},2));
for ii = 1:size(tmp_dane_entropy_norm{1},2)
    res = [];
    for k = 1:length(tmp_dane_entropy_norm)
        res = [res; tmp_dane_entropy_norm{k}(:,ii)];
    end
    weight_entropy(ii) = -(sum(res.*log(res)))/(log(length(res)));
end

weight_entropy = 1 - weight_entropy;

p = find(~isnan(weight_entropy));

weight_entropy = weight_entropy/sum(weight_entropy(p));


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

clear tmp;

for kk = 1:length(data)
  tmp{kk} = zeros(1,size(data{1},2));
  for ii = 1:size(data{1},2)
      res = [];
      for k = 1:length(data)
          res = [res; data{k}(:,ii)];
      end
  %    tmp(ii) = sum(res);
      tmp{kk}(ii) = sqrt(sum(res.*res));
  end
end

dane_norm = cell(1,length(data));
for k = 1:length(data)
    dane_norm{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        dane_norm{k}(:,ii) = data{k}(:,ii)/tmp{k}(ii);
    end
end

saveDataXLS('Normowanie',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

dane_norm_wagi = cell(1,length(dane_norm));
for k = 1:length(dane_norm)
    dane_norm_wagi{k} = zeros(size(dane_norm{k}));
    for ii = 1:size(dane_norm{1},2)
        dane_norm_wagi{k}(:,ii) = dane_norm{k}(:,ii)*wagi2(ii);
    end
end

saveDataXLS('NormowanieWagi',dane_norm_wagi,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);


for kk = 1:length(dane_norm_wagi)
  idealne{kk} = zeros(1,size(dane_norm_wagi{1},2));
  antyidelane{kk} = zeros(1,size(dane_norm_wagi{1},2));
  for ii = 1:size(dane_norm_wagi{1},2)
      res = [];
      res = dane_norm_wagi{kk}(:,ii);
  %    for k = 1:length(dane_norm_wagi)
  %        res = [res;dane_norm_wagi{k}(:,ii)];
  %    end
      if strcmp(charakter(ii),'s') == 1
          idealne{kk}(ii) = max(res);
          antyidelane{kk}(ii) = min(res);
      else
          idealne{kk}(ii) = min(res);
          antyidelane{kk}(ii) = max(res);
      end
  end
end

d_plus = zeros(size(dane_norm_wagi{1},1),1);
d_minus = zeros(size(dane_norm_wagi{1},1),1);

S = cell(1,length(dane_norm_wagi));
for k = 1:length(dane_norm_wagi)
    S{k} = zeros(size(dane_norm_wagi{1},1),2);
    for ii = 1:size(dane_norm_wagi{1},1)
        d_plus(ii) = sqrt(sum(abs((idealne{k}-dane_norm_wagi{k}(ii,:)).^2)));
        d_minus(ii) = sqrt(sum(abs((antyidelane{k}-dane_norm_wagi{k}(ii,:)).^2)));
        S{k}(ii) = d_minus(ii)/(d_plus(ii)+ d_minus(ii));
    end
    S{k}(:,2) = klasyfikacja(S{k}(:,1),liczbaKlas);
end

tmp = cell(1,1);
tmp{1} = 'Wartosc miary';
tmp{2} = 'Klasa';


saveDataXLS(plik_wynikowy,S,lata,tmp,nazwy_wariantow_decyzyjnych,code,[],[]);

