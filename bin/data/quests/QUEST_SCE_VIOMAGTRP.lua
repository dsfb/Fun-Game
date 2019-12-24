QUEST_SCE_VIOMAGTRP = {
	title = 'IDS_PROPQUEST_SCENARIO_INC_000361',
	character = 'MaDa_CloneHachal',
	end_character = 'MaDa_Hachal',
	start_requirements = {
		min_level = 20,
		max_level = 129,
		job = { 'JOB_VAGRANT', 'JOB_MERCENARY', 'JOB_ACROBAT', 'JOB_ASSIST', 'JOB_MAGICIAN', 'JOB_KNIGHT', 'JOB_BLADE', 'JOB_JESTER', 'JOB_RANGER', 'JOB_RINGMASTER', 'JOB_BILLPOSTER', 'JOB_PSYCHIKEEPER', 'JOB_ELEMENTOR', 'JOB_KNIGHT_MASTER', 'JOB_BLADE_MASTER', 'JOB_JESTER_MASTER', 'JOB_RANGER_MASTER', 'JOB_RINGMASTER_MASTER', 'JOB_BILLPOSTER_MASTER', 'JOB_PSYCHIKEEPER_MASTER', 'JOB_ELEMENTOR_MASTER', 'JOB_KNIGHT_HERO', 'JOB_BLADE_HERO', 'JOB_JESTER_HERO', 'JOB_RANGER_HERO', 'JOB_RINGMASTER_HERO', 'JOB_BILLPOSTER_HERO', 'JOB_PSYCHIKEEPER_HERO', 'JOB_ELEMENTOR_HERO' },
	},
	rewards = {
		gold = 15800,
	},
	end_conditions = {
		monsters = {
			{ id = 'MI_VIOLMAGICION', quantity = 10 },
			{ id = 'MI_VIOLMAGICION2', quantity = 1 },
		},
	},
	dialogs = {
		begin = {
			'IDS_PROPQUEST_SCENARIO_INC_000362',
			'IDS_PROPQUEST_SCENARIO_INC_000363',
			'IDS_PROPQUEST_SCENARIO_INC_000364',
			'IDS_PROPQUEST_SCENARIO_INC_000365',
		},
		begin_yes = {
			'IDS_PROPQUEST_SCENARIO_INC_000366',
		},
		begin_no = {
			'IDS_PROPQUEST_SCENARIO_INC_000367',
		},
		completed = {
			'IDS_PROPQUEST_SCENARIO_INC_000368',
		},
		not_finished = {
			'IDS_PROPQUEST_SCENARIO_INC_000369',
		},
	}
}
