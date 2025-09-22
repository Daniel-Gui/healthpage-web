<script lang="ts">
	import HeartHandshake from "@lucide/svelte/icons/heart-handshake";
	import DashboardIcon from "@tabler/icons-svelte/icons/dashboard";
	import HelpIcon from "@tabler/icons-svelte/icons/help";
	import ReportIcon from "@tabler/icons-svelte/icons/report";
	import SettingsIcon from "@tabler/icons-svelte/icons/settings";
	//import NavDocuments from "./nav-documents.svelte";
	import NavMain from "./nav-main.svelte";
	import NavSecondary from "./nav-secondary.svelte";
	import NavUser from "./nav-user.svelte";
	import * as Sidebar from "$lib/components/ui/sidebar/index.js";
	import type { ComponentProps } from "svelte";

	const data = {
		user: {
			name: "Nome do profissional",
			email: "profissional@exemplo.com",
			avatar: "https://blog.sinaxys.com/wp-content/uploads/2024/08/mercado-de-trabalho-da-medicina-medico-sorrindo.jpg",
		},
		navMain: [
			{
				title: "Dashboard",
				url: "/dashboard/healthcare-professional",
				icon: DashboardIcon,
			},
			{
				title: "Agendamentos",
				url: "/dashboard/healthcare-professional/appointments",
				icon: ReportIcon,
			},
		],
		navSecondary: [
			{
				title: "Assinar o Premium",
				url: "#",
				icon: SettingsIcon,
			},
			{
				title: "Configurações",
				url: "#",
				icon: SettingsIcon,
			},
			{
				title: "Suporte",
				url: "#",
				icon: HelpIcon,
			}
		]
	};

	let { ...restProps }: ComponentProps<typeof Sidebar.Root> = $props();
</script>

<Sidebar.Root collapsible="offcanvas" {...restProps}>
	<Sidebar.Header>
		<Sidebar.Menu>
			<Sidebar.MenuItem>
				<Sidebar.MenuButton class="data-[slot=sidebar-menu-button]:!p-1.5">
					{#snippet child({ props })}
						<a href="##" {...props}>
							<HeartHandshake class="!size-5" />
							<span class="text-base font-semibold">HealthPage</span>
						</a>
					{/snippet}
				</Sidebar.MenuButton>
			</Sidebar.MenuItem>
		</Sidebar.Menu>
	</Sidebar.Header>
	<Sidebar.Content>
		<NavMain items={data.navMain} />
		<NavSecondary items={data.navSecondary} class="mt-auto" />
	</Sidebar.Content>
	<Sidebar.Footer>
		<NavUser user={data.user} />
	</Sidebar.Footer>
</Sidebar.Root>
