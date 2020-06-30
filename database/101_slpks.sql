-- table: public.slpks

-- drop table public.slpks;

create table public.slpks
(
    id bigint not null default snow_flake_id(),
    directory character varying(512) collate pg_catalog."default" not null,
    longitude double precision not null,
    latitude double precision not null,
    elevation double precision not null,
    tags character varying(26)[] collate pg_catalog."default",
    creator_id character varying(32) collate pg_catalog."default" not null,
    created_at timestamp without time zone not null,
    updater_id character varying(32) collate pg_catalog."default" not null,
    updated_at timestamp without time zone not null,
    is_deleted boolean not null,
    constraint pk_slpks primary key (id),
    constraint fk_slpks_creator foreign key (creator_id)
        references public.app_users (id) match simple
        on update cascade
        on delete cascade,
    constraint fk_slpks_updator foreign key (updater_id)
        references public.app_users (id) match simple
        on update no action
        on delete no action
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

comment on column public.slpks.tags
    is '标签/别名';

comment on column public.slpks.creator_id
    is '创建者id';

comment on column public.slpks.created_at
    is '创建时间';

comment on column public.slpks.updater_id
    is '更新者id';

comment on column public.slpks.updated_at
    is '更新时间';

comment on column public.slpks.is_deleted
    is '是否删除';

comment on column public.slpks.longitude
    is '模型经度';

comment on column public.slpks.latitude
    is '模型纬度';

comment on column public.slpks.elevation
    is '模型海拔高度';
