clear all

%nazwa_pliku = 'dane-2005-2009';
nazwa_pliku = 'dane_do_obliczen_2016';
%nazwa_pliku = 'dane-2010-2016';
%plik_wynikowy = 'VIKOR_wynik-2005-2009';
%plik_wynikowy = 'VIKOR_wynik-2005-2016';
plik_wynikowy = 'VMCM_wyniki_2016';

liczbaKlas = 4;

p = 1;
 
zmienna = 3;
panstwo = 10;
multiStd = 6;
stepStd = 0.25;



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
%  lata{t} = '2014';t = t + 1;
%  lata{t} = '2015';t = t + 1;
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

tmp = data{1}(:,zmienna);

mean_ = mean(tmp);
std_ = std(tmp);

nn = 0;
for iii = mean_ - multiStd*std_:stepStd*std_:mean_ + multiStd*std_
    nn = nn + 1;
end



tmp0 = zeros(1,size(data{1},2));
tmp1 = zeros(1,size(data{1},2));
tmp2 = zeros(1,size(data{1},2));
for ii = 1:size(data{1},2)
    res = [];
    for k = 1:length(data)
        res = [res; data{k}(:,ii)];
    end
    tmp0(ii) = mean(res);
    tmp2(ii) = std(res);
    s = 0;
    n = 0;
    for k = 1:length(res)
        if abs(res(k)) <= tmp0(ii) + p* tmp2(ii)
            s = s + (res(k) - tmp0(ii)).^2;
            n = n + 1;
        end
    end
    tmp1(ii) = sqrt(s/n);
    
end

dane_norm = cell(1,length(data));
for k = 1:length(data)
    dane_norm{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        dane_norm{k}(:,ii) = (data{k}(:,ii) - tmp0(ii))/tmp1(ii);
    end
end

dane_norm2 = cell(1,length(data));
for k = 1:length(data)
    dane_norm2{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        dane_norm2{k}(:,ii) = (data{k}(:,ii) - tmp2(ii))/tmp1(ii);
    end
end



%saveDataXLS('NormowanieWeight',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);
%saveDataXLS('Normowanie',dane_norm2,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

dane_norm_wagi = cell(1,length(dane_norm));
for k = 1:length(dane_norm)
    dane_norm_wagi{k} = zeros(size(dane_norm{k}));
    for ii = 1:size(dane_norm{1},2)
        dane_norm_wagi{k}(:,ii) = dane_norm{k}(:,ii)*wagi2(ii);
    end
end

dane_norm_wagi2 = cell(1,length(dane_norm2));
for k = 1:length(dane_norm)
    dane_norm_wagi2{k} = zeros(size(dane_norm2{k}));
    for ii = 1:size(dane_norm2{1},2)
        dane_norm_wagi2{k}(:,ii) = dane_norm2{k}(:,ii)*wagi2(ii);
    end
end

%saveDataXLS('NormowanieWeightWagi',dane_norm_wagi,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);
%saveDataXLS('NormowanieWagi',dane_norm_wagi2,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

idealne = zeros(1,size(dane_norm_wagi{1},2));
antyidelane = zeros(1,size(dane_norm_wagi{1},2));
for ii = 1:size(dane_norm_wagi{1},2)
    res = [];
    for k = 1:length(dane_norm_wagi)
        res = [res;dane_norm_wagi{k}(:,ii)];
    end
    tmp = quantile(res,[.25 .75]);
    if strcmp(charakter(ii),'s') == 1
        idealne(ii) = max(res);
        antyidelane(ii) = min(res);
    else
        idealne(ii) = min(res);
        antyidelane(ii) = max(res);
    end
end

idealne2 = idealne - antyidelane; 

S = cell(1,length(dane_norm_wagi));
for k = 1:length(dane_norm_wagi)
    S{k} = zeros(size(dane_norm_wagi{1},1),2);
    for ii = 1:size(dane_norm_wagi{1},1)
        S{k}(ii) = sum((dane_norm_wagi{k}(ii,:) - antyidelane).*idealne2)/sum(idealne2.^2);
    end
    S{k}(:,2) = klasyfikacja(S{k}(:,1),liczbaKlas);
end


tmp = cell(1,1);
tmp{1} = 'Wartosc miary (wa�one odchylenie)';
tmp{2} = 'Klasa';
tmp{3} = 'Wartosc miary';
tmp{4} = 'Klasa';

%saveDataXLS(plik_wynikowy,S,lata,tmp,nazwy_wariantow_decyzyjnych,code,[],[]);

S2_{1} = [S{1}(1:panstwo-1,:);S{1}(panstwo+1:end,:)];

[a,b] = sort(S2_{1}(:,1),'descend');

for ii = 1:panstwo-1
    code2{ii} = code{ii};
    nazwy_wariantow_decyzyjnych2{ii} = nazwy_wariantow_decyzyjnych{ii};
end
for ii = panstwo + 1:length(nazwy_wariantow_decyzyjnych)
    nazwy_wariantow_decyzyjnych2{ii - 1} = nazwy_wariantow_decyzyjnych{ii};
    code2{ii-1} = code{ii};
end

for ii = 1:length(nazwy_wariantow_decyzyjnych2)
    nazwy_wariantow_decyzyjnych3{ii} = nazwy_wariantow_decyzyjnych2{b(ii)};
    code3{ii} = code2{b(ii)};
end

S2_{1}(:,1) = S2_{1}(b,1);
%S2_{1}(:,2) = 0;
[a,b_] = sort(S2_{1}(:,1),'descend');



start = 0;
iiii = 0;

for iii = mean_ - multiStd*std_:stepStd*std_:mean_ + multiStd*std_
    data{1}(panstwo,zmienna) = iii;


    
    tmp0 = zeros(1,size(data{1},2));
    tmp1 = zeros(1,size(data{1},2));
    tmp2 = zeros(1,size(data{1},2));
    for ii = 1:size(data{1},2)
        res = [];
        for k = 1:length(data)
            res = [res; data{k}(:,ii)];
        end
        tmp0(ii) = mean(res);
        tmp2(ii) = std(res);
        s = 0;
        n = 0;
        for k = 1:length(res)
            if abs(res(k)) <= tmp0(ii) + p* tmp2(ii)
                s = s + (res(k) - tmp0(ii)).^2;
                n = n + 1;
            end
        end
        tmp1(ii) = sqrt(s/n);

    end

    dane_norm = cell(1,length(data));
    for k = 1:length(data)
        dane_norm{k} = zeros(size(data{k}));
        for ii = 1:size(data{1},2)
            dane_norm{k}(:,ii) = (data{k}(:,ii) - tmp0(ii))/tmp1(ii);
        end
    end

    dane_norm2 = cell(1,length(data));
    for k = 1:length(data)
        dane_norm2{k} = zeros(size(data{k}));
        for ii = 1:size(data{1},2)
            dane_norm2{k}(:,ii) = (data{k}(:,ii) - tmp2(ii))/tmp1(ii);
        end
    end



    %saveDataXLS('NormowanieWeight',dane_norm,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);
    %saveDataXLS('Normowanie',dane_norm2,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

    dane_norm_wagi = cell(1,length(dane_norm));
    for k = 1:length(dane_norm)
        dane_norm_wagi{k} = zeros(size(dane_norm{k}));
        for ii = 1:size(dane_norm{1},2)
            dane_norm_wagi{k}(:,ii) = dane_norm{k}(:,ii)*wagi2(ii);
        end
    end

    dane_norm_wagi2 = cell(1,length(dane_norm2));
    for k = 1:length(dane_norm)
        dane_norm_wagi2{k} = zeros(size(dane_norm2{k}));
        for ii = 1:size(dane_norm2{1},2)
            dane_norm_wagi2{k}(:,ii) = dane_norm2{k}(:,ii)*wagi2(ii);
        end
    end

    %saveDataXLS('NormowanieWeightWagi',dane_norm_wagi,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);
    %saveDataXLS('NormowanieWagi',dane_norm_wagi2,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

    idealne = zeros(1,size(dane_norm_wagi{1},2));
    antyidelane = zeros(1,size(dane_norm_wagi{1},2));
    for ii = 1:size(dane_norm_wagi{1},2)
        res = [];
        for k = 1:length(dane_norm_wagi)
            res = [res;dane_norm_wagi{k}(:,ii)];
        end
        tmp = quantile(res,[.25 .75]);
        if strcmp(charakter(ii),'s') == 1
            idealne(ii) = max(res);
            antyidelane(ii) = min(res);
        else
            idealne(ii) = min(res);
            antyidelane(ii) =  max(res);
        end
    end

    idealne2 = idealne - antyidelane; 

    if start == 0
        S_ = cell(1,length(dane_norm_wagi));
    end
    for k = 1:length(dane_norm_wagi)
        if start == 0
            S_{k} = zeros(size(dane_norm_wagi{1},1),2*nn);
        end
        for ii = 1:size(dane_norm_wagi{1},1)
            S_{k}(ii,1 + iiii*2) = sum((dane_norm_wagi{k}(ii,:) - antyidelane).*idealne2)/sum(idealne2.^2);
        end
        S_{k}(:,2 + iiii*2) = klasyfikacja(S_{k}(:,1 + iiii*2),liczbaKlas);
    end
    
    start = 1;
    iiii = iiii + 1;
end

tmp = cell(1,1);
tmp{1} = 'Wartosc miary';
tmp{2} = 'Klasa';


S2{1} = [S_{1}(1:panstwo-1,:);S_{1}(panstwo+1:end,:)];

for ii = 1:2:size(S2{1},2)
    S2{1}(:,ii) = S2{1}(b,ii);
    [a,b2] = sort(S2{1}(:,ii),'descend');
    S2{1}(:,ii + 1) = abs(b2 - b_);
end

saveDataXLS('wynik',S2,lata,tmp,nazwy_wariantow_decyzyjnych3,code3,[],[]);

clear tmp;
s = mean_ - multiStd*std_;
s2 = -multiStd;

jj = 1;
for ii = 1:2:size(S2{1},2)
    tmp{1,jj} = s; 
    tmp{2,jj} = s2; 
    tmp{3,jj} = sum(S2{1}(:,ii + 1));
    tmp{4,jj} = sum(abs(S2{1}(:,ii) - S2_{1}(:,1))); 
    s = s + stepStd*std_;
    s2 = s2 + stepStd;
    jj = jj + 1;
end

xlswrite('wynik2.xls',tmp);
