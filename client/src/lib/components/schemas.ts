import { z } from "zod/v4";

export const schema = z.object({
	id: z.number(),
	patientName: z.string(),
	appointmentType: z.string(),
	status: z.string(),
	date: z.string(),
	time: z.string(),
	duration: z.string(),
	doctorName: z.string(),
});

export type Schema = z.infer<typeof schema>;
