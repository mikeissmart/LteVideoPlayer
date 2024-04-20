//     This code was generated by a Reinforced.Typings tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.

export interface IConvertFileDto
{
	id?: number;
	output?: string;
	errored?: boolean;
	originalFile?: IFileDto;
	convertedFile?: IFileDto;
	createdDate?: Date;
	startedDate?: Date;
	endedDate?: Date;
}
export interface ICreateConvertDto
{
	originalFile?: IFileDto;
	convertedFile?: IFileDto;
}
export interface IDirDto
{
	dirPath?: string;
	dirName?: string;
	dirPathName?: string;
}
export interface IDirsAndFilesDto
{
	dirs?: IDirDto[];
	files?: IFileDto[];
}
export interface IFileDto
{
	filePath?: string;
	fileName?: string;
	fileExists?: boolean;
	convertQueued?: boolean;
	filePathName?: string;
}
export interface IFileHistoryDto
{
	id?: number;
	userProfileId?: number;
	percentWatched?: number;
	fileEntity?: IFileDto;
	startedDate?: Date;
}
export interface IRemoteData
{
	profile?: string;
	channel?: number;
}
export interface IRemoteData_MoveSeekDto extends IRemoteData
{
	seekPosition?: number;
}
export interface IRemoteData_PauseDto extends IRemoteData
{
}
export interface IRemoteData_PlayDto extends IRemoteData
{
}
export interface IRemoteData_SetSeekDto extends IRemoteData
{
	seekPercentPosition?: number;
}
export interface IRemoteData_SetVolumeDto extends IRemoteData
{
	volume?: number;
}
export interface IRemoteData_VideoInfoDto extends IRemoteData
{
	videoFile?: string;
	currentTimeSeconds?: number;
	maxTimeSeconds?: number;
	volume?: number;
	isPlaying?: boolean;
}
export interface IStringDto
{
	data?: string;
}
export interface IUserProfileDto
{
	id?: number;
	name?: string;
	createdDate?: Date;
}
