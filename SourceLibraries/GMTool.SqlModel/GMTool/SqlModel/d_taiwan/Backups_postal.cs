using System;
using SqlSugar;

namespace GMTool.SqlModel.d_taiwan;

[SugarTable("Backups_postal")]
public class Backups_postal
{
	[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
	public int postal_id { get; set; }

	public DateTime occ_time { get; set; }

	public int send_charac_no { get; set; }

	public string send_charac_name { get; set; }

	public int receive_charac_no { get; set; }

	public int item_id { get; set; }

	public int add_info { get; set; }

	public short endurance { get; set; }

	public byte upgrade { get; set; }

	public byte amplify_option { get; set; }

	public int amplify_value { get; set; }

	public int gold { get; set; }

	public DateTime receive_time { get; set; }

	public byte delete_flag { get; set; }

	public byte avata_flag { get; set; }

	public byte unlimit_flag { get; set; }

	public byte seal_flag { get; set; }

	public byte creature_flag { get; set; }

	public int postal { get; set; }

	public int letter_id { get; set; }

	public int extend_info { get; set; }

	public byte ipg_db_id { get; set; }

	public int ipg_transaction_id { get; set; }

	public string ipg_nexon_id { get; set; }

	public long auction_id { get; set; }

	public byte[] random_option { get; set; }

	public byte seperate_upgrade { get; set; }

	public byte type { get; set; }

	public byte[] item_guid { get; set; }

	public DateTime? DataTime { get; set; }

	public int? FromID { get; set; }
}
