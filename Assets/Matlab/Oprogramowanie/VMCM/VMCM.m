clear all

%nazwa_pliku = 'dane-2005-2009';
nazwa_pliku = 'dane_do_obliczen_2016.xlsx';
%nazwa_pliku = 'dane-2010-2016';
%plik_wynikowy = 'VIKOR_wynik-2005-2009';
%plik_wynikowy = 'VIKOR_wynik-2005-2016';
plik_wynikowy = 'VMCM_wyniki';

liczbaKlas = 4;

p = 100000;
 
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


weight3 = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    weight3(ii) = std(res)/mean(res);
end

weight3 = weight3/sum(weight3);

%wagi2 = weight3;

for kk = 1:length(data)
  tmp0{kk} = zeros(1,size(data{1},2));
  %tmp1 = zeros(1,size(data{1},2));
  tmp2{kk} = zeros(1,size(data{1},2));
  for ii = 1:size(data{1},2)
      res = [];
      res = data{kk}(:,ii);
  %    tmp0(ii) = mean(res);
  %    tmp2(ii) = std(res);
      tmp0{kk}(ii) = 0;
      tmp2{kk}(ii) = sum(res.^2);
  %    s = 0;
  %    n = 0;
  %    for k = 1:length(res)
  %        if abs(res(k)) <= tmp0(ii) + p* tmp2(ii)
  %            s = s + (res(k) - tmp0(ii)).^2;
  %            n = n + 1;
  %        end
  %    end
  %    tmp1(ii) = sqrt(s/n);
      
  end
end

%dane_norm = cell(1,length(data));
%for k = 1:length(data)
%    dane_norm{k} = zeros(size(data{k}));
%    for ii = 1:size(data{1},2)
%        dane_norm{k}(:,ii) = (data{k}(:,ii) - tmp0(ii))/tmp1(ii);
%    end
%end

dane_norm2 = cell(1,length(data));
for k = 1:length(data)
    dane_norm2{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        dane_norm2{k}(:,ii) = (data{k}(:,ii) - tmp0{k}(ii))/tmp2{k}(ii);
    end
end



%saveDataXLS('NormowanieWeight',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);
saveDataXLS('Normowanie',dane_norm2,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

%dane_norm_wagi = cell(1,length(dane_norm));
%for k = 1:length(dane_norm)
%    dane_norm_wagi{k} = zeros(size(dane_norm{k}));
%    for ii = 1:size(dane_norm{1},2)
%        dane_norm_wagi{k}(:,ii) = dane_norm{k}(:,ii)*wagi2(ii);
%    end
%end

dane_norm_wagi2 = cell(1,length(dane_norm2));
for k = 1:length(dane_norm2)
    dane_norm_wagi2{k} = zeros(size(dane_norm2{k}));
    for ii = 1:size(dane_norm2{1},2)
        dane_norm_wagi2{k}(:,ii) = dane_norm2{k}(:,ii)*wagi2(ii);
    end
end

%saveDataXLS('NormowanieWeightWagi',dane_norm_wagi,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);
saveDataXLS('NormowanieWagi',dane_norm_wagi2,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);


%%%%%

for kk = 1:length(data)
  idealne_{kk} = zeros(1,size(dane_norm_wagi2{1},2));
  antyidelane_{kk} = zeros(1,size(dane_norm_wagi2{1},2));
  for ii = 1:size(dane_norm_wagi2{1},2)
      res = [];
      res = dane_norm_wagi2{k}(:,ii);
%      for k = 1:length(dane_norm_wagi2)
%          res = [res;dane_norm_wagi2{k}(:,ii)];
%      end
      tmp = quantile(res,[.25 .75]);
      if strcmp(charakter(ii),'s') == 1
          idealne_{kk}(ii) = tmp(2);
          antyidelane_{kk}(ii) = tmp(1);
      else
          idealne_{kk}(ii) = tmp(1);
          antyidelane_{kk}(ii) = tmp(2);
      end
  end

  idealne2_{kk} = idealne_{kk} - antyidelane_{kk}; 
end

for k = 1:length(dane_norm_wagi2)
%    S{k + length(dane_norm_wagi2)} = zeros(size(dane_norm_wagi2{1},1),2);
    for ii = 1:size(dane_norm_wagi2{1},1)
%        S{k + length(dane_norm_wagi2)}(ii) = sum((dane_norm_wagi2{k}(ii,:) - antyidelane).*idealne2)/sum(idealne2.^2);
        S{k}(ii,3) = sum((dane_norm_wagi2{k}(ii,:) - antyidelane_{k}).*idealne2_{k})/sum(idealne2_{k}.^2);
    end
    S{k}(:,4) = klasyfikacja(S{k}(:,3),liczbaKlas);
end


tmp = cell(1,1);
tmp{1} = 'Wartosc miary (wa¿one odchylenie)';
tmp{2} = 'Klasa';
tmp{3} = 'Wartosc miary';
tmp{4} = 'Klasa';

saveDataXLS(plik_wynikowy,S,lata,tmp,nazwy_wariantow_decyzyjnych,code,[],[]);


