-- table: public.json_data

-- drop table public.json_data;

create table public.json_data
(
    id bigint not null,
    value jsonb not null,
    constraint pk_json_data primary key (id)
)
with (
    oids = false
)
tablespace pg_default;

alter table public.json_data
    owner to postgres;

comment on table public.json_data
    is 'json 数据';

comment on column public.json_data.id
    is 'json 数据id';

comment on column public.json_data.value
    is 'json值';
