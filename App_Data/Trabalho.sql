--create table clientes(
--  idcli int identity(1,1) primary key,
--  nome nvarchar(50) not null unique,
--  datanasc datetime ,
--  idade as datediff(year,datanasc, getdate()),
--  categoria nvarchar(50) check(categoria in('alfa','bravo','charlie')),
--  tutor int foreign key references clientes(idcli)
--);

--insert into clientes(nome,datanasc,categoria)
--values
--('Tio Patinhas','1900-01-01', 'alfa'),
--('Pato Donald','1903-01-01', 'bravo'),
--('Margarida','1945-01-01', 'alfa'),
--('Peninha','1930-01-01', 'alfa');
--select * from clientes;

--create table carros (
--  idcar int identity primary key,
--  modelo nvarchar(50)not null,
--  phora decimal(10,2) check(phora >=0)

--);
--insert into carros (modelo, phora)
--values
--('fiat 600', 20.50),
--('ford fiesta', 30.50),
--('mercedes cla', 120.00),
--('ferrari', 350.50);
--select * from carros;
--alter table carros add xpto int;
--alter table carros alter column xpto float;
--sp_help 'carros'; 
--alter table carros add constraint xx  check(xpto>0);
--alter table carros  drop column xpto; 
--alter table carros drop constraint xx;

--alter table clientes add photopath nvarchar(255);

--create table alugueres (
-- idal int identity primary key,
-- idcli int ,
-- constraint fkaluguercliente foreign key (idcli) references clientes(idcli)
-- on update cascade on delete cascade,
-- idcar int foreign key references carros(idcar),
-- inicio datetime default getdate(),
-- fim  datetime ,
-- tempo as cast (datediff(minute, inicio,fim)/60 as decimal(10,2)),
-- custo decimal(10,2),
-- constraint ual unique(idcli,idcar,inicio)
--);

--select * from alugueres;

--select * from clientes;
update clientes set tutor=(
	case idcli
	when 1 then NULL
	when 2 then 1
	else 2
end
);

--select * from alugueres;

insert into alugueres (idcli,idcar,inicio,fim)
select c.idcli, ca.idcar,getdate(),dateadd(minute,300,getdate())
						 from clientes c, carros ca ;

--delete from alugueres where idcli =4;
--delete from alugueres where idcli =2 and idcar <= 2;

select * from alugueres;

update alugueres set custo = a.tempo * ca.phora
	from alugueres a inner join carros ca on ca.idcar=a.idcar ;

delete from clientes where idcli =4;

--select * from clientes;
--set identity_insert clientes ON;
--insert into clientes (idcli,nome,datanasc,categoria,tutor)
--values(4,'Marga Min', '1956-01-01', 'bravo', 3);
--set identity_insert clientes OFF;

-- Algebra relacional

--select
	-- Mostra os clientes cujo começa por M ou T

		select * from clientes where nome like '[MT]%';
		select * from clientes where nome like '[^MT]%'; -- Mostra os clientes que não começam com M ou T

	-- Mostra os clientes quem têm 6 letras no nome

		select * from clientes where nome like '______'; -- Pode se colocar logo uma letra para
														 -- procurar nomes com essa letra, ex: 'M________'
	-- Mostra o 3º e 5º clientes mais velhos

		select * from (select *, row_number() over (order by datanasc asc)
		as posicao from clientes) as ranked where posicao in (3, 5);

--project
	-- Projeta o campo e apelido e os anos quem faltam para fazer 50 anos

		select dbo.fn_apelido(nome) as Apelido, 50 - datediff(year, datanasc, getdate())
		as AnosFaltam from clientes;

--rename
	-- Mostra os arranjos, sem e com repetição dos clientes dois a dois

		-- Sem repetição

			select a.nome, b.nome from clientes a, clientes b where a.idcli < b.idcli;

		-- Com repetições

			select a.nome, b.nome from clientes a, clientes b where a.idcli <> b.idcli;

	-- Faz combinações

		select a.nome, b.nome from clientes a, clientes b;

--cross product 
--join
	-- Mostra o nome do cliente e o modelo do carro por aluguer

		select c.nome, ca.modelo from alugueres a
		inner join clientes c on c.idcli = a.idcli
		inner join carros ca on ca.idcar = a.idcar;

--left join / right join / full outer join
	-- Mostra os clientes que não alugaram carros

		-- Left join

			select c.* from clientes c left join alugueres a
			on a.idcli = c.idcli where a.idcli is null;

		-- Rigth join

			select c.* from alugueres a right join clientes c
			on c.idcli = a.idcli where a.idcli is null;

		-- Full outer join

			select c.* from alugueres a full outer join clientes c
			on c.idcli = a.idcli where a.idcli is null;

--semi-join
	-- Mostra os clientes que alugaram carros

		select distinct c.* from clientes c
		inner join alugueres a on a.idcli = c.idcli;

--anti join
	-- Mostra os clientes que não alugaram carros

		select * from clientes where idcli not in (select idcli from alugueres);

--intersect
	-- Mostra os alugueres dos clientes 1 e 3

		select idcar from alugueres where idcli = 1 intersect
		select idcar from alugueres where idcli = 3;

--union / union all
	-- Mostra a hierarquia dos clientes por tutor

		select c.nome as Cliente, t.nome as Tutor from clientes c
		left join clientes t on t.idcli = c.tutor;

--except
	-- Mostra os clientes que já alugaram algum carro mas não alugaram todos

		select idcli from alugueres except select idcli from clientes c
		where (select count(distinct idcar) from alugueres where idcli = c.idcli) 
		= (select count(*) from carros);

--division
	-- Mostra os clientes que já alugaram os carros todos

		select idcli from clientes c where not exists (select idcar from carros except
		select idcar from alugueres where idcli = c.idcli);

--aggregation
	-- Mostra a soma e média de custos por cliente

		select c.nome, sum(a.custo) as Total, avg(a.custo) as Media from alugueres a
		inner join clientes c on c.idcli = a.idcli group by c.idcli, c.nome;

--update / update from / delete / insert

-- Programação
-- function
	-- Cria uma função que devolva o apelido do cliente

		GO
			create function fn_apelido(@nome nvarchar(max))returns nvarchar(max)
			AS
			BEGIN
				declare  @pos int;
				declare @apelido nvarchar(max);
				set @pos = charindex(' ', reverse (@nome));
				if @pos <= 0 set @apelido = @nome;
				else set @apelido = right(@nome,@pos);
				return @apelido;
			END
		GO

	-- Função de um número aleatório

		select dbo.fn_apelido('José Neves');
		select *, dbo.fn_apelido(nome) as Apelido from clientes;

		GO
			create view vw_sorte
			AS
			select rand() as sorte;

			
		GO
			create function fn_sorte(@min int, @max int)returns int
			AS
			BEGIN
				declare @delta int = @max - @min +1;

				return floor((select sorte from vw_sorte)*@delta)+ @min;
			END
		GO
		select dbo.fn_sorte(1,50);


-- trigger
	-- Crie um trigger que atualize o custo do aluguer no insert e no delete

		GO
		create trigger trg_custo
		on alugueres
		after insert, update
		AS
		BEGIN
			update alugueres set custo = a.tempo * ca.phora from alugueres a
			inner join carros ca on ca.idcar = a.idcar inner join inserted i on i.idal = a.idal;
		END
		GO

-- procedure
	-- Mostre os alugueres, quantidade e soma dos custos de um cliente

		GO
		create procedure sp_alugueresCliente @idcli int
		AS
		BEGIN
			select a.idal, ca.modelo, a.inicio, a.fim, a.custo from alugueres a
			inner join carros ca on ca.idcar = a.idcar where a.idcli = @idcli;

			select count(*) as Quantidade, sum(custo) as TotalCustos
			from alugueres where idcli = @idcli;
		END
		GO

		exec sp_alugueresCliente 1;

		--GO
		--	create trigger custoaluguer on alugueres
		--	after insert , update
		--	AS
		--	Begin
		--		update alugueres set custo = i.tempo * ca.phora from
		--			alugueres a inner join carros.ca on ca.idcar = a.idcar
		--			inner join inserted.i on i.idal = a.idal;
		--	End
		--GO

		--go
		--	create trigger custoaluguer on alugueres
		--	after insert , update
		--	AS
		--	Begin
		--		update alugueres set custo=i.tempo * ca.phora from
		--			   alugueres a inner join carros ca on ca.idcar=a.idcar
		--			   inner join inserted i on i.idal=a.idal;
		--	End
		--go