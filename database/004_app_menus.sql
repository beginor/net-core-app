
drop table if exists public.application_menus;

/*==============================================================*/
/* Table: application_menus                                     */
/*==============================================================*/
create table application_menus (
   id                   INT8                 not null default snow_flake_id(),
   name                 VARCHAR(50)          null,
   full_name            VARCHAR(1000)        null,
   url                  VARCHAR(200)         null,
   parent_id            INT8                 null,
   full_url             VARCHAR(1000)        null,
   page_code            VARCHAR(200)         null,
   moduel_code          VARCHAR(200)         null,
   level                int4                 null,
   order_no             INT4                 null,
   summary_source       VARCHAR(50)          null,
   created_user         VARCHAR(50)          null,
   created_time         DATE                 null,
   updated_user         VARCHAR(50)          null,
   updated_time         DATE                 null,
   is_view              BOOL                 null,
   constraint PK_APPLICATION_MENUS primary key (id)
);

comment on table application_menus is
'系统菜单表';

comment on column application_menus.id is
'雪花ID（snow_flake_id)';

comment on column application_menus.name is
'菜单简称';

comment on column application_menus.full_name is
'所有父级菜单简称拼接，使用“/”间隔';

comment on column application_menus.url is
'菜单URL';

comment on column application_menus.parent_id is
'父级菜单ID';

comment on column application_menus.full_url is
'菜单ID拼接，使用“|”间隔';

comment on column application_menus.page_code is
'页面路由编码';

comment on column application_menus.moduel_code is
'路由名称，权限module';

comment on column application_menus.level is
'等级,从1开始';

comment on column application_menus.order_no is
'菜单序列号';

comment on column application_menus.summary_source is
'用于统计计数名称的数据源';

comment on column application_menus.created_user is
'创建人姓名';

comment on column application_menus.created_time is
'创建时间';

comment on column application_menus.updated_user is
'修改人姓名';

comment on column application_menus.updated_time is
'修改时间';

comment on column application_menus.is_view is
'是否在界面上显示';
