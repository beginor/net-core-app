-- add creator_id, created_at, updater_id, updated_at, is_deleted to xxx table;
-- xxx is your table name, please replace it with your table name.

alter table public.xxx
    add column creator_id character varying(32) not null;
alter table public.xxx
    add column created_at timestamp without time zone not null;
alter table public.xxx
    add column updater_id character varying(32) not null;
alter table public.xxx
    add column updated_at timestamp without time zone not null;
alter table public.xxx
    add column is_deleted boolean not null;

comment on column public.xxx.creator_id
    is '创建者id';
comment on column public.xxx.created_at
    is '创建时间';
comment on column public.xxx.updater_id
    is '更新者id';
comment on column public.xxx.updated_at
    is '更新时间';
comment on column public.xxx.is_deleted
    is '是否删除';

alter table public.xxx
    add constraint fk_xxx_creator foreign key (creator_id)
    references public.app_users (id) match simple
    on update cascade
    on delete cascade;
alter table public.xxx
    add constraint fk_xxx_updater foreign key (updater_id)
    references public.app_users (id) match simple
    on update no action
    on delete no action;

alter table public.xxx
    rename constraint xxx_pkey to pk_xxx;
