-- table: public.slpks

-- drop table public.slpks;

create table public.slpks
(
    id bigint not null default snow_flake_id(),
    directory character varying(512) collate pg_catalog."default" not null,
    longitude double precision not null,
    latitude double precision not null,
    elevation double precision not null,
    constraint pk_slpks primary key (id)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.slpks
    owner to postgres;
comment on table public.slpks
    is 'slpk 航拍模型';

comment on column public.slpks.id
    is '航拍模型id';

comment on column public.slpks.directory
    is '航拍模型目录';

comment on column public.slpks.longitude
    is '模型经度';

comment on column public.slpks.latitude
    is '模型纬度';

comment on column public.slpks.elevation
    is '模型海拔高度';
