-- table: public.categories

-- drop table if exists public.categories;

create table if not exists public.categories (
    id bigint not null default snow_flake_id(),
    name character varying(32) collate pg_catalog."default" not null,
    parent_id bigint,
    sequence real not null default 1.0,
    constraint pk_categories primary key (id),
    constraint fk_categories_parent_id foreign key (parent_id)
        references public.categories (id) match simple
        on update cascade
        on delete cascade
        not valid
)

tablespace pg_default;

alter table if exists public.categories
    owner to postgres;

comment on table public.categories
    is '数据类别';

comment on column public.categories.id
    is '类别ID';

comment on column public.categories.name
    is '类别名称';

comment on column public.categories.parent_id
    is '父类ID';

comment on column public.categories.sequence
    is '顺序号';
-- index: fki_categories_parent_id

-- drop index if exists public.fki_categories_parent_id;

create index if not exists fki_categories_parent_id
    on public.categories using btree
    (parent_id asc nulls last)
    tablespace pg_default;
