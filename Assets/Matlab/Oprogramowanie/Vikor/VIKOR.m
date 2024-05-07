clear all

%nazwa_pliku = 'dane-2005-2009';
nazwa_pliku = 'dane_do_obliczen_2016';
%nazwa_pliku = 'dane-2010-2016';
%plik_wynikowy = 'VIKOR_wynik-2005-2009';
%plik_wynikowy = 'VIKOR_wynik-2005-2016';
plik_wynikowy = 'VIKOR_wyniki';

liczbaKlas = 4;

q=0.6;
 
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

wagi2 = weight3;


for kk = 1:length(data)
  idealne{kk} = zeros(1,size(data{1},2));
  antyidelane{kk} = zeros(1,size(data{1},2));
  for ii = 1:size(data{1},2)
      res = [];
      res = data{kk}(:,ii);
%      for k = 1:length(data)
%          res = [res;data{k}(:,ii)];
%      end
      if strcmp(charakter(ii),'s') == 1
          idealne{kk}(ii) = max(res);
          antyidelane{kk}(ii) = min(res);
      else
          idealne{kk}(ii) = min(res);
          antyidelane{kk}(ii) = max(res);
      end
  end
end


r = cell(1,length(data));
for k = 1:length(data)
    r{k} = zeros(size(data{k}));
    for ii = 1:size(data{1},2)
        r{k}(:,ii) = (idealne{kk}(ii) - data{k}(:,ii))/(idealne{kk}(ii) - antyidelane{kk}(ii));
    end
end

saveDataXLS('Normowanie',r,lata,zmienne,nazwy_wariantow_decyzyjnych,code,[],color);

S = cell(1,length(r));
R = cell(1,length(r));

for k = 1:length(r)
    S{k} = zeros(size(data{1},1),1);
    R{k} = zeros(size(data{1},1),1);
    for ii = 1:size(r{1},1)
        pom = r{k}(ii,:).*wagi;
        S{k}(ii) = sum(pom);
        R{k}(ii) = max(pom);
    end
end

Q = cell(1,length(r));

for k = 1:length(r)
    Q{k} = zeros(size(data{1},1),1);
    for ii = 1:size(r{1},1)
        Q{k}(ii) = q*(S{k}(ii) - min(S{k}))/(max(S{k}) - min(S{k})) + (1-q)*(R{k}(ii) - min(R{k}))/(max(R{k}) - min(R{k}));
    end
end

res = cell(1,length(Q));
style = cell(1,length(Q));
color = cell(1,length(Q));
for k = 1:length(Q)
    res{k} = cell(3,4);
    style{k} = cell(3,4);
    color{k} = cell(3,4);
    res{k}{1,1} = 'Obiekt';
    res{k}{1,2} = 'Kod';
    res{k}{1,3} = 'Q';
    res{k}{1,4} = 'Klasa';
    res{k}{1,5} = 'Wynik';
    style{k}{1,1} = 's';
    style{k}{1,2} = 's';
    style{k}{1,3} = 's';
    style{k}{1,4} = 's';
    color{k}{1,1} = [];
    color{k}{1,2} = [];
    color{k}{1,3} = [];
    color{k}{1,4} = [];
    
    [Qa Qb] = sort(Q{k});
    [Ra Rb] = sort(R{k});
    [Sa Sb] = sort(S{k});
    
    klass = 5-klasyfikacja(Qa,liczbaKlas);

 
    DQ = 1/(length(Q{k})-1);

    war1 = 0;
    if Qa(2) - Qa(1) >= DQ
        war1 = 1;
    end

    war2 = 0;
    if Qb(1) == Rb(1) && Qb(1) == Sb(1)
        war2 = 1;
    end
    
    if war1==1 && war2 == 1
        res{k}{2,1} = nazwy_wariantow_decyzyjnych{Qb(1)};
        style{k}{2,1} = 's';
        color{k}{2,1} = [];
        res{k}{2,2} = code{Qb(1)};
        style{k}{2,2} = 's';
        color{k}{2,2} = [];
        res{k}{2,5} = 'Rozwi¹zanie najlepsze';
        style{k}{2,5} = 's';
        color{k}{2,5} = [];
        res{k}{2,3} = Qa(1);
        style{k}{2,3} = 'd';
        color{k}{2,3} = [];
        res{k}{2,4} = klass(1);
        style{k}{2,4} = 'd';
        color{k}{2,4} = [];
        for ii=2:length(Q{1})
            res{k}{ii+1,1} = nazwy_wariantow_decyzyjnych{Qb(ii)};
            style{k}{ii+1,1} = 's';
            color{k}{ii+1,1} = [];
            res{k}{ii+1,2} = code{Qb(ii)};
            style{k}{ii+1,2} = 's';
            color{k}{ii+1,2} = [];
            res{k}{ii+1,5} = 'Inne rozwi¹zanie';
            style{k}{ii+1,5} = 's';
            color{k}{ii+1,5} = [];
            res{k}{ii+1,3} = Qa(ii);
            style{k}{ii+1,3} = 'd';
            color{k}{ii+1,3} = [];
            res{k}{ii+1,4} = klass(ii);
            style{k}{ii+1,4} = 'd';
            color{k}{ii+1,4} = [];
        end
    elseif war2 == 1
        res{k}{2,1} = nazwy_wariantow_decyzyjnych{Qb(1)};
        style{k}{2,1} = 's';
        color{k}{2,1} = [];
        res{k}{2,2} = code{Qb(1)};
        style{k}{2,2} = 's';
        color{k}{2,2} = [];
        res{k}{2,5} = 'Rozwi¹zanie najlepsze';
        style{k}{2,5} = 's';
        color{k}{2,5} = [];
        res{k}{2,3} = Qa(1);
        style{k}{2,3} = 'd';
        color{k}{2,3} = [];
        res{k}{2,4} = klass(1);
        style{k}{2,4} = 'd';
        color{k}{2,4} = [];

        res{k}{3,1} = nazwy_wariantow_decyzyjnych{Qb(2)};
        style{k}{3,1} = 's';
        color{k}{3,1} = [];
        res{k}{3,2} = code{Qb(2)};
        style{k}{3,2} = 's';
        color{k}{3,2} = [];
        res{k}{3,5} = 'Rozwi¹zanie kompromisowe';
        style{k}{3,5} = 's';
        color{k}{3,5} = [];
        res{k}{3,3} = Qa(2);
        style{k}{3,3} = 'd';
        color{k}{3,3} = [];
        res{k}{3,4} = klass(2);
        style{k}{3,4} = 'd';
        color{k}{3,4} = [];

        for ii=3:length(Q{1})
            res{k}{ii+1,1} = nazwy_wariantow_decyzyjnych{Qb(ii)};
            style{k}{ii+1,1} = 's';
            color{k}{ii+1,1} = [];
            res{k}{ii+1,2} = code{Qb(ii)};
            style{k}{ii+1,2} = 's';
            color{k}{ii+1,2} = [];
            res{k}{ii+1,5} = 'Inne rozwi¹zanie';
            style{k}{ii+1,5} = 's';
            color{k}{ii+1,5} = [];
            res{k}{ii+1,3} = Qa(ii);
            style{k}{ii+1,3} = 'd';
            color{k}{ii+1,3} = [];
            res{k}{ii+1,4} = klass(ii);
            style{k}{ii+1,4} = 'd';
            color{k}{ii+1,4} = [];
        end
    
    else
        res{k}{2,1} = nazwy_wariantow_decyzyjnych{Qb(1)};
        style{k}{2,1} = 's';
        color{k}{2,1} = [];
        res{k}{2,2} = code{Qb(1)};
        style{k}{2,2} = 's';
        color{k}{2,2} = [];
        res{k}{2,5} = 'Rozwi¹zanie najlepsze';
        style{k}{2,5} = 's';
        color{k}{2,5} = [];
        res{k}{2,3} = Qa(1);
        style{k}{2,3} = 'd';
        color{k}{2,3} = [];
        res{k}{2,4} = klass(1);
        style{k}{2,4} = 'd';
        color{k}{2,4} = [];
        
        res{k}{3,1} = nazwy_wariantow_decyzyjnych{Qb(2)};
        style{k}{3,1} = 's';
        color{k}{3,1} = [];
        res{k}{3,2} = code{Qb(2)};
        style{k}{3,2} = 's';
        color{k}{3,2} = [];
        res{k}{3,5} = 'Rozwi¹zanie kompromisowe';
        style{k}{3,5} = 's';
        color{k}{3,5} = [];
        res{k}{3,3} = Qa(2);
        style{k}{3,3} = 'd';
        color{k}{3,3} = [];
        res{k}{3,4} = klass(2);
        style{k}{3,4} = 'd';
        color{k}{3,4} = [];
        tmp='Rozwi¹zanie kompromisowe';
        for ii=3:length(Q{1})
            res{k}{ii+1,1} = nazwy_wariantow_decyzyjnych{Qb(ii)};
            style{k}{ii+1,1} = 's';
            color{k}{ii+1,1} = [];
            res{k}{ii+1,2} = code{Qb(ii)};
            style{k}{ii+1,2} = 's';
            color{k}{ii+1,2} = [];
            res{k}{ii+1,5} = tmp;
            style{k}{ii+1,5} = 's';
            color{k}{ii+1,5} = [];
            res{k}{ii+1,3} = Qa(ii);
            style{k}{ii+1,3} = 'd';
            color{k}{ii+1,3} = [];
            res{k}{ii+1,4} = klass(ii);
            style{k}{ii+1,4} = 'd';
            color{k}{ii+1,4} = [];
            if Qb(ii) - Qb(1) < DQ
                tmp = 'Inne rozwi¹zanie';
            end
        end
        
    end    
    
end

saveXLS(plik_wynikowy,lata,res,style,color);


