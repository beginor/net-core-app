-- table: public.vectortiles

-- drop table public.vectortiles;

create table public.vectortiles
(
    id bigint not null default snow_flake_id(),
    directory character varying(512) collate pg_catalog."default" not null,
    min_zoom smallint,
    max_zoom smallint,
    default_style character varying(32),
    min_latitude numeric(10, 8),
    max_latitude numeric(10, 8),
    min_longitude numeric(11, 8),
    max_longitude numeric(11, 8),
    constraint pk_vectortiles primary key (id)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.vectortiles
    owner to postgres;

comment on table public.vectortiles
    is '矢量切片包';

comment on column public.vectortiles.id
    is '矢量切片包id';

comment on column public.vectortiles.directory
    is '矢量切片包目录';

comment on column public.vectortiles.min_zoom
    is '最小缩放级别';

comment on column public.vectortiles.max_zoom
    is '最大缩放级别';

comment on column public.vectortiles.default_style
    is '默认样式';

comment on column public.vectortiles.min_latitude
    is '最小纬度';

comment on column public.vectortiles.max_latitude
    is '最大纬度';

comment on column public.vectortiles.min_longitude
    is '最小经度';

comment on column public.vectortiles.max_longitude
    is '最大经度';
