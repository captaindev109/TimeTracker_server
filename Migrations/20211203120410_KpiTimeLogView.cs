using Microsoft.EntityFrameworkCore.Migrations;

namespace TimeTracker_server.Migrations
{
  public partial class KpiTimeLogView : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.Sql(@"
      CREATE OR REPLACE VIEW public.kpi_timelog
        AS
        SELECT v.parentId AS objectId,
            v.parentType AS objectType,
            v.companyId,
            ur.id AS userId,
            sum(((tt.end - tt.start) - tt.pauseDuration)) AS value_time_total,
            sum(
                CASE
                    WHEN (date_trunc(wee::text, tt.date) = date_trunc(week::text, now())) THEN ((tt.end - tt.start) - tt.pauseDuration)
                    ELSE (0)::bigint
                END) AS value_time_current_month
        FROM ((acl_vertical_lvl_2 v
            JOIN TimeTables tt ON ((v.childId = tt.id)))
            JOIN Users ur ON ((1 = 1)))
        GROUP BY v.parentId, v.parentType, v.companyId, ur.id;

        ALTER TABLE public.kpi_timelog
            OWNER TO postgres;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
        drop view public.kpi_timelog;
        ");
    }
  }
}
